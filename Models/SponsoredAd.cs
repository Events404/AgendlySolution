using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SponsoredAd
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; } 
        public decimal Price { get; set; }
        public bool IsPaid { get; set; } 
        public string UserId { get; set; } 
        public int EventId { get; set; } 
        public DateTime CreatedAt { get; set; }

        public Event Event { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}
