using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility.ViewModel;

namespace Agendly.Controllers
{
    public class EventReportController : Controller
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<SponsoredAd> _sponsoredAdRepository;

        public EventReportController(EventIRepository eventRepository, SponsoredAdIRepository sponsoredAdRepository)
        {
            _eventRepository = eventRepository;
            _sponsoredAdRepository = sponsoredAdRepository;
        }

        // صفحة التقرير الخاصة بالحدث
        public IActionResult EventReport(int eventId)
        {
            // جلب بيانات الحدث باستخدام الـ Repository
            var eventDetails = _eventRepository.GetOne(e => e.Id == eventId);

            if (eventDetails == null)
            {
                return NotFound(); // في حالة عدم وجود الحدث
            }

            // جلب بيانات الإعلانات الممولة الخاصة بالحدث
            var sponsoredAds = _sponsoredAdRepository.Get(expression: sa => sa.EventId == eventId).ToList();

            // إنشاء موديل يحتوي على تفاصيل الحدث مع عدد الزيارات والنقرات بالإضافة إلى الإعلانات الممولة
            var reportViewModel = new EventReportViewModel
            {
                EventDetails = eventDetails,
                SponsoredAds = sponsoredAds,
                ViewCount = eventDetails.ViewCount
            };

            return View(reportViewModel);
        }
    }
}



