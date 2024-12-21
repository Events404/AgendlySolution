using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class SponsoredAdViewModel
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Ad Title is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The title must be between 2 and 100 characters.")]
        public string Title { get; set; } // The title of the event

        [Required(ErrorMessage = "Ad description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "The description must be between 10 and 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 700000, ErrorMessage = "The price must be between 1 and 700,000 EGP.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ad duration is required.")]
        [Range(1, 70, ErrorMessage = "The duration must be between 1 and 70 days.")]
        public int Duration { get; set; } // Duration in days

        public string ClientReferenceId { get; set; } // Optional client reference ID
    }
}
