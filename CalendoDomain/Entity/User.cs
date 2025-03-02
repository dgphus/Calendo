using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class User : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Schedule>? Schedules { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<Participant>? Participants { get; set; }
        public ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
