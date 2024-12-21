using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
 
        public class AdminReportViewModel
        {

            public List<SponsoredAdReportViewModel> SponsoredAds { get; set; }
            public decimal TodayEarnings { get; set; }
            public decimal WeeklyEarnings { get; set; }
            public decimal MonthlyEarnings { get; set; }
        

        }

}

