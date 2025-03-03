using CalendoApplication.ViewModels.Request.Auth;
using CalendoApplication.ViewModels.Response.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.IService.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest loginDTO);
        Task<LoginResponse> RefreshToken(string refreshToken);
        Task<bool> RegisterUser(RegisterRequest registerRequest);
        Task<bool> ConfirmEmailAsync(string token, string email);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
