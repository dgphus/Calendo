using CalendoApplication.IService.Auth;
using CalendoApplication.ViewModels.Request.Auth;
using CoreApiResponse;
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
    }
}
