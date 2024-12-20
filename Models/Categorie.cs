﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Categorie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(16, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 16 characters.")]
        public string Name { get; set; }
        [ValidateNever]
        public List<Event> Events { get; set; }
    }
}
