using CalendoDomain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class Schedule
    {
        [Key]
        public Guid ScheduleId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ScheduleStatus? Status { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<Reminder>? Reminders { get; set; }
        public ICollection<Participant>? Participants { get; set; }
        public ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
