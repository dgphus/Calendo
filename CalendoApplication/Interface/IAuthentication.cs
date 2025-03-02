using CalendoDomain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.Interface
{
    public interface IAuthentication
    {
        Task<string> GenerateJWTToken(User user);
        Guid GetUserIdFromHttpContext(HttpContext httpContext);

        Task<bool> VerifyPassword(string providedPassword, string hashedPassword, User user);

        string HashPassword(User user, string password);

        string GenerateRefreshToken();

        Task SaveRefreshToken(User user, string refreshToken);

        Task<bool> ValidateRefreshToken(User user, string refreshToken);

        Task<(string newJwtToken, string newRefreshToken)> RefreshToken(User user, string refreshToken);
    }
}
