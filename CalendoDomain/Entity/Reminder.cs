using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class Reminder
    {
        [Key]
        public Guid ReminderId { get; set; } = Guid.NewGuid();
        public Guid ScheduleId { get; set; }
        public DateTime ReminderTime { get; set; }
        public string? Message { get; set; }

        public Schedule? Schedule { get; set; }
    }
}
