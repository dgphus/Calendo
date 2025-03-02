using CalendoApplication.Interface;
using CalendoDomain.CustomExeption;
using CalendoDomain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CalendoInfrastructure.Authentication
{
    public class Authen : IAuthentication
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Authen> _logger;

        public Authen(IConfiguration configuration, IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<Authen> logger)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger; _userManager = userManager;
        }

        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public Task<bool> VerifyPassword(string providedPassword, string hashedPassword, User user)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return Task.FromResult(result == PasswordVerificationResult.Success);
        }
        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public async Task<string> GenerateJWTToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(user);

            var userClaims = await _userManager.GetClaimsAsync(user);
            var farmIdClaim = userClaims.FirstOrDefault(c => c.Type == "FarmId")?.Value ?? string.Empty;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, string.Join(",", roles)),
                new Claim("avatar",user.Avatar),
                new Claim("farmId", farmIdClaim)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task SaveRefreshToken(User user, string refreshToken)
        {
            // Lưu refresh token vào bảng thông qua UserManager
            await _userManager.SetAuthenticationTokenAsync(user, "FarmerOnline", "RefreshToken", refreshToken);
        }

        // Xác thực refresh token
        public async Task<bool> ValidateRefreshToken(User user, string refreshToken)
        {
            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "FarmerOnline", "RefreshToken");

            if (storedToken == null || storedToken != refreshToken)
            {
                return false; // Token không hợp lệ
            }

            return true; // Token hợp lệ
        }

        // Làm mới JWT và refresh token
        public async Task<(string newJwtToken, string newRefreshToken)> RefreshToken(User user, string refreshToken)
        {
            // Kiểm tra refresh token có hợp lệ hay không
            var isValidRefreshToken = await ValidateRefreshToken(user, refreshToken);

            if (!isValidRefreshToken)
            {
                throw new CustomException.ForbbidenException("Refresh Token không hợp lệ.");
            }

            // Tạo mới JWT token
            var newJwtToken = await GenerateJWTToken(user);

            // Tạo và lưu refresh token mới
            var newRefreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user, newRefreshToken);

            return (newJwtToken, newRefreshToken);
        }

        public Guid GetUserIdFromHttpContext(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                throw new CustomException.InternalServerErrorException("Authorization header is missing.");
            }

            string authorizationHeader = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new CustomException.InternalServerErrorException("Invalid Authorization header format.");
            }

            string jwtToken = authorizationHeader["Bearer ".Length..];

            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(jwtToken))
            {
                throw new CustomException.InternalServerErrorException("Invalid JWT token format.");
            }

            try
            {
                var token = tokenHandler.ReadJwtToken(jwtToken);
                var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "id");

                if (idClaim == null || string.IsNullOrWhiteSpace(idClaim.Value))
                {
                    throw new CustomException.InternalServerErrorException("User ID claim not found in token.");
                }

                return Guid.Parse(idClaim.Value);
            }
            catch (Exception ex)
            {
                throw new CustomException.InternalServerErrorException($"Error parsing token: {ex.Message}");
            }
        }
    }
}
