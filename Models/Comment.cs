using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Comment must be between 2 and 16 characters.")]
        [Display(Name = "Comment")]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

    }
}
