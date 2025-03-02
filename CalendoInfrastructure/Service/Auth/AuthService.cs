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

        public AuthService(
            IAuthentication authentication, IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration configuration)
        {
            _authentication = authentication;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
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
    }
}
