using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class EventReportViewModel
    {
        public Event EventDetails { get; set; }
        public List<SponsoredAd> SponsoredAds { get; set; }
        public int ViewCount { get; set; }
    }
}
