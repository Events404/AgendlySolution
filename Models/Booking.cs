﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
     public class Booking
    {
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }
        
        public string Status { get; set; } 
        public int EventId { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }


    }
}
