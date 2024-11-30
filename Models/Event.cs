using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agendly.Models;


namespace Models
{
    public class Event
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 16 characters.")]
        [Display(Name = "Event Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 1000 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }= DateTime.Now;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 16 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "City must be between 2 and 16 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Distric is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Distric must be between 2 and 16 characters.")]
        [Display(Name = "Distric")]
        public string Distric { get; set; }
        [Required(ErrorMessage = "Street is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 16 characters.")]
        [Display(Name = "Street")]
        public string Street { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }
        public string UrlImg { get; set; }
        [Required(ErrorMessage = "Location URL is required.")]
        [Url(ErrorMessage = "Please provide a valid URL.")]
        public string location { get; set; }
        public double? Rate { get; set; }
        public int? Like { get; set; }
        public int? DisLike { get; set; }
        public int CategorieId { get; set; }
        public Categorie categorie { get; set; }

        public List<PromoCode> PromoCodes { get; set; }
        public List<Comment> comments  { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

    }
}
