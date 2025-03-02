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
    }
}
