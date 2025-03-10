﻿using CalendoDomain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoDomain.Entity
{
    public class Participant
    {
        [Key]
        public Guid ParticipantId { get; set; } = Guid.NewGuid();
        public Guid SchedulesId { get; set; }
        public Guid UserId { get; set; }
        public ParticipantType? Status { get; set; }

        public Schedule? Schedule { get; set; }
        public User? User { get; set; }
    }
}
