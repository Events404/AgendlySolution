using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 16 characters.")]
        [Display(Name = "First Name")]

        public String FirstName { get; set; }
        [Required(ErrorMessage = "List Name is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "List Name must be between 2 and 16 characters.")]
        [Display(Name = "List Name")]
        public String LastName { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 16 characters.")]
        [Display(Name = "Country")]
        public String Country { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "City must be between 2 and 16 characters.")]
        [Display(Name = "City")]
        public String City { get; set; }
        public byte[]? ProfilePicture { get; set; }

        public List<Booking> Bookings { get; set; } 
        public List<WishList> WishLists { get; set; } 
        public List<Event> Events { get; set; }

    }
}
