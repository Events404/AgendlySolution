using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Home
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]

        public string WelcomeText { get; set; }
        [Required]
        [StringLength(300)]

        public string AboutAgendly { get; set; }
        [Required]
        [StringLength(300)]

        public string TaskManagementDescription { get; set; }
        [Required]
        [StringLength(300)]

        public string TeamCollaborationDescription { get; set; }
        [Required]
        [StringLength(300)]

        public string PerformanceInsightsDescription { get; set; }

        public List<FAQ> FAQs { get; set; }

    }
}  
