using CalendoApplication.Mapper;
using CalendoDomain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.ViewModels.Request.Auth
{
    public class LoginRequest : IMapFrom<User>
    {
        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required]
        public string? Password { get; set; } = string.Empty;
    }
}
