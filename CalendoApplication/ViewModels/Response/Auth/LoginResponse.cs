using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.ViewModels.Response.Auth
{
    public class LoginResponse()
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
