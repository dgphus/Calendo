using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class Attachment
    {
        [Key]
        public Guid AttachmentId { get; set; } = Guid.NewGuid();
        public Guid ScheduleId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public Schedule? Schedule { get; set; }
    }

}
