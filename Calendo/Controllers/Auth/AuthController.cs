using CalendoApplication.IService.Auth;
using CalendoApplication.ViewModels.Request.Auth;
using CoreApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendoAPI.Controllers.Auth
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {

            var results = await _authService.Login(request);
            return CustomResult("Đăng nhập thành công.", results);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequest request)
        {

            var results = await _authService.RefreshToken(request.refreshToken);
            return CustomResult("Refresh thành công.", results);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var results = await _authService.RegisterUser(request);
            return CustomResult("Tạo tài khoản thành công. Vui lòng kiểm tra email để xác nhận tài khoản trước khi đăng nhập.", results);
        }

        [HttpPost("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var isConfirmed = await _authService.ConfirmEmailAsync(request.Token, request.Email);
            return CustomResult("Xác nhận email thành công.", isConfirmed);

        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {

            await _authService.ForgotPasswordAsync(request.Email);
            return CustomResult("Email khôi phục mật khẩu đã được gửi.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {

            await _authService.ResetPasswordAsync(request);
            return CustomResult("Đặt lại mật khẩu thành công.");

        }
    }
}
