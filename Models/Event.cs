using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Models
{
    public class Event
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 16 characters.")]
        [Display(Name = "Event Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 16 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [ValidateNever]
        public string? StartDateHumanized { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "City must be between 2 and 16 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "District is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "District must be between 2 and 16 characters.")]
        [Display(Name = "District")]
        public string District { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 16 characters.")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }
        [ValidateNever]
        public string UrlImg { get; set; }

        [Required(ErrorMessage = "Location URL is required.")]
        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string Location { get; set; }

        [ValidateNever]
        public int CategorieId { get; set; }

        [ValidateNever]
        public Categorie Categorie { get; set; }

        [ValidateNever]
        public List<PromoCode> PromoCodes { get; set; }

        [ValidateNever]
        public List<Comment> Comments { get; set; }

        [ValidateNever]
        public string UserId { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }

        public int Seats { get; set; }
        [ValidateNever]
        public bool Available { get; set; }
        [ValidateNever]
        public List<LikeDislike>? LikeDislikes { get; set; }
    }
}
