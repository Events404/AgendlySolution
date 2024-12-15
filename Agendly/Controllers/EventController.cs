using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models;
using System.Linq.Expressions;
using DataAccess.Repository;
using Humanizer;
using Utility.ViewModel;

namespace Agendly.Controllers
{
    public class EventController : Controller
    {
        private readonly EventIRepository eventIRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly CategoryIRepository categoryIRepository;
        private readonly CommentIRepository commentIRepository;
        private readonly LikeDisLikeIRepository likeDisLikeIRepository;
        private readonly NotificationIRepository notificationIRepository;

        public EventController(EventIRepository eventIRepository, UserManager<ApplicationUser> userManager, CategoryIRepository categoryIRepository, CommentIRepository commentIRepository , LikeDisLikeIRepository likeDisLikeIRepository , NotificationIRepository notificationIRepository)
        {
            this.eventIRepository = eventIRepository;
            this.userManager = userManager;
            this.categoryIRepository = categoryIRepository;
            this.commentIRepository = commentIRepository;
            this.likeDisLikeIRepository = likeDisLikeIRepository;
            this.notificationIRepository = notificationIRepository;
        }



        public IActionResult Index(string filter = "all", string searchQuery = "", int page = 1, int pageSize = 10)
        {
            var events = eventIRepository.Get(
                expression: e => e.User != null,
                includeProp: new Expression<Func<Event, object>>[] { e => e.User }
             ).ToList();
            foreach (var eventItem in events)
            {
                eventItem.StartDateHumanized = eventItem.StartDate.Humanize(); 
            }

            if (filter == "available")
            {
                events = events.Where(e => e.StartDate > DateTime.Now && e.EndDate > DateTime.Now).ToList(); 
            }
            else if (filter == "past")
            {
                events = events.Where(e => e.EndDate < DateTime.Now).ToList(); 
            }
            else if (filter == "today")
            {
                events = events.Where(e => e.StartDate.Date == DateTime.Now.Date).ToList(); 
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                events = events.Where(e => e.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                           e.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalEvents = events.Count();
            var totalPages = (int)Math.Ceiling((double)totalEvents / pageSize);

            events = events.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new EventIndexViewModel
            {
                Events = events,
                CurrentPage = page,
                TotalPages = totalPages,
                Filter = filter,
                SearchQuery = searchQuery 
            };

            return View(model);
        }

        public IActionResult IndexAdmin(string filter = "all", string searchQuery = "", int page = 1, int pageSize = 10)
        {
            var events = eventIRepository.Get(
                expression: e => e.User != null,
                includeProp: new Expression<Func<Event, object>>[] { e => e.User }
            ).ToList(); 

            foreach (var eventItem in events)
            {
                eventItem.StartDateHumanized = eventItem.StartDate.Humanize(); 
            }

            if (filter == "available")
            {
                events = events.Where(e => e.StartDate > DateTime.Now && e.EndDate > DateTime.Now).ToList(); 
            }
            else if (filter == "past")
            {
                events = events.Where(e => e.EndDate < DateTime.Now).ToList(); 
            }
            else if (filter == "today")
            {
                events = events.Where(e => e.StartDate.Date == DateTime.Now.Date).ToList(); 
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                events = events.Where(e => e.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                           e.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalEvents = events.Count();
            var totalPages = (int)Math.Ceiling((double)totalEvents / pageSize);

            events = events.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new EventIndexViewModel
            {
                Events = events,
                CurrentPage = page,
                TotalPages = totalPages,
                Filter = filter,
                SearchQuery = searchQuery 
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            var categories = categoryIRepository.GatAll();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event modelevent, IFormFile UrlImg)
        {
            if (!ModelState.IsValid)
            {
                var categories = categoryIRepository.GatAll();
                ViewBag.Categories = categories;
                return View(modelevent);
            }

            if (!User.Identity.IsAuthenticated)
            {
                TempData["error"] = "Please log in first.";
                var loginUrl = Url.Action("Login", "Account", new { area = "Identity" });
                return Redirect(loginUrl);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            modelevent.UserId = user.Id;
            modelevent.CreationDate = DateTime.Now;

            if (modelevent.StartDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("StartDate", "Event start date cannot be in the past.");
            }

            if (modelevent.EndDate.Date < modelevent.StartDate.Date)
            {
                ModelState.AddModelError("EndDate", "Event end date cannot be earlier than the start date.");
            }

            if (!ModelState.IsValid)
            {
                var categories = categoryIRepository.GatAll();
                ViewBag.Categories = categories;
                return View(modelevent);
            }

            if (UrlImg != null && UrlImg.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(UrlImg.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("UrlImg", "Invalid file type. Allowed types are .jpg, .jpeg, .png, and .gif.");
                    var categories = categoryIRepository.GatAll();
                    ViewBag.Categories = categories;
                    return View(modelevent);
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgEvent", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UrlImg.CopyToAsync(stream);
                }
                modelevent.UrlImg = fileName;
            }
            else
            {
                ModelState.AddModelError("UrlImg", "Event image is required.");
                var categories = categoryIRepository.GatAll();
                ViewBag.Categories = categories;
                return View(modelevent);
            }

            if (modelevent.Seats <= 0)
            {
                ModelState.AddModelError("Seats", "Number of seats must be greater than zero.");
                var categories = categoryIRepository.GatAll();
                ViewBag.Categories = categories;
                return View(modelevent);
            }

            modelevent.Available = modelevent.Seats > 0 && modelevent.StartDate > DateTime.Now;

            if (modelevent.StartDate > DateTime.Now && modelevent.Seats > 0)
            {
                modelevent.Available = true;
            }
            else
            {
                modelevent.Available = false;
            }

            eventIRepository.create(modelevent);
            eventIRepository.commit();

            TempData["success"] = "Event created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var eventItem = eventIRepository.GetFirstOrDefault(
                expression: e => e.Id == id,
                includeProp: new Expression<Func<Event, object>>[]
                {
            e => e.Categorie,
            e => e.User,
            e => e.Comments
                }
            );

            if (eventItem == null)
            {
                return NotFound();
            }

            eventItem.Available = eventItem.Seats > 0 && eventItem.StartDate > DateTime.Now;

            return View(eventItem);
        }


        [HttpPost]
        public IActionResult ReserveSeats(int id, int numberOfSeats)
        {
            var eventItem = eventIRepository.GetFirstOrDefault(
                expression: e => e.Id == id,
                includeProp: new Expression<Func<Event, object>>[]
                {
            e => e.Categorie,
            e => e.User
                }
            );

            if (eventItem == null)
            {
                return Json(new { success = false, message = "Event not found." });
            }

            if (!eventItem.Available)
            {
                return Json(new { success = false, message = "Event is not available for reservation." });
            }

            if (numberOfSeats <= 0 || numberOfSeats > eventItem.Seats)
            {
                return Json(new { success = false, message = "Invalid number of seats." });
            }

            eventItem.Seats -= numberOfSeats;
            eventIRepository.Edit(eventItem);
            eventIRepository.commit();

            return Json(new { success = true, message = $"Successfully reserved {numberOfSeats} seats." });
        }
        public IActionResult Delete(int id)
        {
            var eventItem = eventIRepository.GetFirstOrDefault(e => e.Id == id);
            if (eventItem == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Index");
            }
            var commentsCount = commentIRepository.GatAll().Count(c => c.EventId == id);
            ViewBag.CommentsCount = commentsCount;
            eventIRepository.Delete(eventItem);
            eventIRepository.commit();

            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction("Event", "Create");
        }

        public async Task<IActionResult> MyEvents(int page = 1, int pageSize = 10)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var events = eventIRepository.Get(
                expression: e => e.UserId == user.Id,  
                includeProp: new Expression<Func<Event, object>>[] { e => e.User }
            ).ToList();

            foreach (var eventItem in events)
            {
                eventItem.StartDateHumanized = eventItem.StartDate.Humanize(); 
            }

            var totalEvents = events.Count();
            var totalPages = (int)Math.Ceiling((double)totalEvents / pageSize);

            events = events.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new EventIndexViewModel
            {
                Events = events,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int eventId, string commentContent)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Please log in to add a comment." });
            }

            if (string.IsNullOrWhiteSpace(commentContent) || commentContent.Length < 2 || commentContent.Length > 16)
            {
                return Json(new { success = false, message = "Comment must be between 2 and 16 characters." });
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var comment = new Comment
            {
                Content = commentContent,
                EventId = eventId,
                UserId = user.Id,
                CreationDate = DateTime.Now,
                User = user
            };

            var commentsCount = commentIRepository.Count();
            ViewBag.CommentsCount = commentsCount;

            commentIRepository.create(comment);
            commentIRepository.commit();

            return Json(new { success = true, userName = user.FirstName + " " + user.LastName, postedDate = comment.CreationDate.ToString("dd MMM yyyy HH:mm"), commentText = comment.Content });
        }

        [HttpPost]
        public async Task<IActionResult> AddLikeDislike(int eventId, bool isLike)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Please log in to like or dislike." });
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var existingLikeDislike = likeDisLikeIRepository.GetByUserAndEvent(user.Id, eventId);

            if (existingLikeDislike != null)
            {
                likeDisLikeIRepository.Delete(existingLikeDislike);
                likeDisLikeIRepository.commit();

                var likesDislikes = likeDisLikeIRepository.Get(null, ld => ld.EventId == eventId);
                var likeCount = likesDislikes.Count(ld => ld.IsLike == true);
                var dislikeCount = likesDislikes.Count(ld => ld.IsLike == false);

                ViewBag.LikeCount = likeCount;
                ViewBag.DislikeCount = dislikeCount;

                return Json(new { success = true, message = "Action removed successfully." });
            }
            else
            {
                var newLikeDislike = new LikeDislike
                {
                    UserId = user.Id,
                    EventId = eventId,
                    IsLike = isLike
                };

                likeDisLikeIRepository.create(newLikeDislike);
                likeDisLikeIRepository.commit();

                var likesDislikes = likeDisLikeIRepository.Get(null, ld => ld.EventId == eventId);
                var likeCount = likesDislikes.Count(ld => ld.IsLike == true);
                var dislikeCount = likesDislikes.Count(ld => ld.IsLike == false);

                ViewBag.LikeCount = likeCount;
                ViewBag.DislikeCount = dislikeCount;

                return Json(new { success = true, message = isLike ? "Liked successfully." : "Disliked successfully." });
            }
        }


    }
}
