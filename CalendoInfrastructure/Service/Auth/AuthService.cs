using AutoMapper;
using CalendoApplication.Interface;
using CalendoApplication.IService.Auth;
using CalendoApplication.ViewModels.Request.Auth;
using CalendoApplication.ViewModels.Response.Auth;
using CalendoDomain.CustomExeption;
using CalendoDomain.Entity;
using CalendoDomain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoInfrastructure.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthentication _authentication;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        private readonly UserManager<User> _userManager;

        public AuthService(
            IAuthentication authentication, IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration configuration, ISendMail sendMail, UserManager<User> userManager)
        {
            _authentication = authentication;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _sendMail = sendMail;
            _userManager = userManager;
        }

        public async Task<LoginResponse> Login(LoginRequest loginDTO)
        {

            var user = await _unitOfWork.GetRepository<User>()
                .Entities.Where(u => u.Email == loginDTO.Email && !u.LockoutEnabled).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new CustomException.InvalidDataException("Email hoặc mật khẩu không hợp lệ.");
            }

            if (!user.EmailConfirmed)
            {
                throw new CustomException.ForbbidenException("Tài khoản chưa được xác nhận. Vui lòng kiểm tra email để xác nhận.");
            }

            var currentTime = CoreHelper.SystemTimeNow;

            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > currentTime)
            {
                var remainingLockoutTime = user.LockoutEnd.Value - currentTime;
                throw new CustomException.ForbbidenException($"Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau {remainingLockoutTime.TotalMinutes:N0} phút.");
            }

            if (!await _authentication.VerifyPassword(loginDTO.Password, user.PasswordHash, user))
            {

                user.AccessFailedCount++;

                if (user.AccessFailedCount >= 3)
                {
                    user.LockoutEnd = currentTime.AddMinutes(15);
                    await _unitOfWork.SaveAsync();
                    throw new CustomException.ForbbidenException("Bạn đã đăng nhập sai quá số lần quy định. Tài khoản đã bị khóa trong 15 phút.");
                }

                await _unitOfWork.SaveAsync();
                throw new CustomException.InvalidDataException("Email hoặc mật khẩu không hợp lệ.");
            }

            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _unitOfWork.SaveAsync();

            string token = await _authentication.GenerateJWTToken(user);
            string refreshToken = _authentication.GenerateRefreshToken();
            await _authentication.SaveRefreshToken(user, refreshToken);

            return new LoginResponse { token = token, refreshToken = refreshToken };
        }

        public async Task<LoginResponse> RefreshToken(string refreshToken)
        {
            var users = await _unitOfWork.GetRepository<User>().Entities.ToListAsync();

            User user = null;

            foreach (var u in users)
            {
                var storedToken = await _userManager.GetAuthenticationTokenAsync(u, "FarmerOnline", "RefreshToken");
                if (storedToken == refreshToken)
                {
                    user = u;
                    break;
                }
            }

            // Kiểm tra nếu không tìm thấy người dùng với refresh token hợp lệ
            if (user == null)
            {
                throw new CustomException.ForbbidenException("Refresh token không hợp lệ.");
            }

            // Tạo JWT token mới
            var newJwtToken = await _authentication.GenerateJWTToken(user);

            // Tạo refresh token mới và lưu lại
            var newRefreshToken = _authentication.GenerateRefreshToken();
            await _authentication.SaveRefreshToken(user, newRefreshToken);

            return new LoginResponse
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            };
        }

        public async Task<bool> RegisterUser(RegisterRequest registerRequest)
        {
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                throw new CustomException.InvalidDataException("Password và ConfirmPassword không trùng khớp.");
            }

            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                throw new CustomException.InvalidDataException("Email đã tồn tại trong hệ thống.");
            }

            var user = _mapper.Map<User>(registerRequest);
            user.Email = user.UserName = registerRequest.Email;
            user.Avatar = "https://lanit.com.vn/wp-content/uploads/2023/11/c-sharp-la-gi-10.png";
            user.EmailConfirmed = false;

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new CustomException.InvalidDataException($"Đăng ký thất bại: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                throw new CustomException.InvalidDataException("Gán vai trò thất bại.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationUrlTemplate = _configuration["Mail:ConfirmationUrl"];
            var callbackUrl = string.Format(confirmationUrlTemplate, token, registerRequest.Email);
            await _sendMail.SendConfirmationEmailAsync(registerRequest.Email, callbackUrl);

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string token, string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                throw new CustomException.InvalidDataException("Invalid email confirmation request.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new CustomException.InvalidDataException($"Không tìm thấy người dùng với email {email}.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrlTemplate = _configuration["Mail:PasswordResetUrl"];
            var callbackUrl = string.Format(resetUrlTemplate, token, email);

            await _sendMail.SendForgotPasswordEmailAsync(email, callbackUrl);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new CustomException.InvalidDataException("Mật khẩu mới và xác nhận mật khẩu không giống nhau.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            return result.Succeeded;
        }
    }
}
