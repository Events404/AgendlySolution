using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace Agendly.Controllers
{
    public class EventController : Controller
    {
        private readonly EventIRepository eventIRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public EventController(EventIRepository eventIRepository , UserManager<ApplicationUser> userManager)
        {
            this.eventIRepository = eventIRepository;
            this.userManager = userManager;
        }
         public IActionResult Index()
        {
            var events = eventIRepository.GatAll(include: "User");
            return View(events);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                eventModel.UserId = userId;

                eventIRepository.create(eventModel);
                eventIRepository.commit();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
