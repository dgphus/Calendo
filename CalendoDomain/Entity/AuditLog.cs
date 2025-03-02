using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class AuditLog
    {
        [Key]
        public Guid AuditId { get; set; } = Guid.NewGuid();
        public Guid ScheduleId { get; set; }
        public Guid UserId { get; set; }
        public string? Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public Schedule? Schedule { get; set; }
        public User? User { get; set; }
    }

}
