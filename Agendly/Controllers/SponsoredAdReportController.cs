using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using Utility.ViewModel;
using System;
using System.Threading.Tasks;
using Stripe;

namespace Agendly.Controllers
{
    public class SponsoredAdReportController : Controller
    {
        private readonly IRepository<SponsoredAd> sponsoredAdRepository;
        private readonly EventIRepository eventRepository;
        private readonly IRepository<ApplicationUser> userRepository;

        public SponsoredAdReportController(IRepository<SponsoredAd> sponsoredAdRepository,
                                     EventIRepository eventRepository,
                                     IRepository<ApplicationUser> userRepository)
        {
            this.sponsoredAdRepository = sponsoredAdRepository;
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
        }

        public IActionResult Index()
        {
            var sponsoredAds = sponsoredAdRepository.Get(
                includeProp: new System.Linq.Expressions.Expression<Func<SponsoredAd, object>>[]
                {
                    ad => ad.Event,
                    ad => ad.Event.User
                });

            var today = DateTime.Today;
            var thisWeekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var thisMonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var todayEarnings = sponsoredAds
                .Where(ad => ad.CreatedAt.Date == today && ad.IsPaid)
                .Sum(ad => ad.Price);

            var weeklyEarnings = sponsoredAds
                .Where(ad => ad.CreatedAt >= thisWeekStart && ad.IsPaid)
                .Sum(ad => ad.Price);

            var monthlyEarnings = sponsoredAds
                .Where(ad => ad.CreatedAt >= thisMonthStart && ad.IsPaid)
                .Sum(ad => ad.Price);

            var reportViewModel = new AdminReportViewModel
            {
                SponsoredAds = sponsoredAds.Select(ad => new SponsoredAdReportViewModel
                {
                    Id = ad.Id,
                    Title = ad.Title,
                    Description = ad.Description,
                    Duration = ad.Duration,
                    Price = ad.Price,
                    IsPaid = ad.IsPaid,
                    UserId = ad.UserId,
                    EventId = ad.EventId,
                    CreatedAt = ad.CreatedAt,
                    Event = ad.Event,
                    User = ad.Event?.User,
                    CanRefund = ad.IsPaid // إضافة خاصية لإظهار زر إرجاع الأموال
                }).ToList(),

                TodayEarnings = todayEarnings,
                WeeklyEarnings = weeklyEarnings,
                MonthlyEarnings = monthlyEarnings
            };

            return View(reportViewModel);
        }

      
    }
}
