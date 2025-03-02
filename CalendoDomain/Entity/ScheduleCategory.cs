using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class ScheduleCategory
    {
        [Key]
        public Guid ScheduleCategoryId { get; set; }
        public Guid CategoryId { get; set; }

        public Schedule? Schedule { get; set; }
        public Category? Category { get; set; }
    }
}
