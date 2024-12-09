using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class AddUserViewModel
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

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<RoleViewModel> Roles { get; set; }

    }
}
