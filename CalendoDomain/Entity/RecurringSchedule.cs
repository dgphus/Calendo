using CalendoDomain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class RecurringSchedule
    {
        [Key]
        public Guid RecurringId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public RecurrenceType? RecurrenceType { get; set; }
        public DateTime? EndDate { get; set; }

        public User? User { get; set; }
    }
}