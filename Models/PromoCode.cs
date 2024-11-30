using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agendly.Models;


namespace Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        [StringLength(50, ErrorMessage = "Code must be less than 50 characters.")]
        public string Code { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount Amount must be greater than zero.")]
        public decimal? DiscountAmount { get; set; }
        [Range(0, 100, ErrorMessage = "Discount Percentage must be between 0 and 100.")]
        public decimal? DiscountPercentage { get; set; }
        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Usage Limit is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Usage Limit must be greater than zero.")]
        public int UsageLimit { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int EventId { get; set; }
        public Event Event { get; set; }


    }
}
