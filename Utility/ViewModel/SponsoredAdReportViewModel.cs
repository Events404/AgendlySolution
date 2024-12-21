using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class SponsoredAdReportViewModel
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
        public bool CanRefund {  get; set; }
        // Related Entities
        public Event Event { get; set; }
        public ApplicationUser User { get; set; }
    }

}
