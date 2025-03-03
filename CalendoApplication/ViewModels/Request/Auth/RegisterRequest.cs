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
    public class RegisterRequest : IMapFrom<User>
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập password.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập confirm password.")]
        public string ConfirmPassword { get; set; }
    }
}
