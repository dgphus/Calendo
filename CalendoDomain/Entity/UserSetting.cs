using CalendoDomain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class UserSetting
    {
        [Key]
        public Guid SettingId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public ThemeType? Theme { get; set; }
        public string? Language { get; set; }
        public bool? NotificationEnabled { get; set; }

        public User? User { get; set; }
    }
}
