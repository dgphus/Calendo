using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class Log
    {
        [Key]
        public Guid LogId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string? Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
