using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Stripe;
using Stripe.Checkout;
using Utility.ViewModel;

namespace Agendly.Controllers
{
    public class SponsoredAdController : Controller
    {
        private readonly SponsoredAdIRepository _sponsoredAdRepository;
        private readonly EventIRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public SponsoredAdController(
            SponsoredAdIRepository sponsoredAdRepository,
            EventIRepository eventRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _sponsoredAdRepository = sponsoredAdRepository;
            _eventRepository = eventRepository;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        // GET: SponsoredAd/Create/{eventId}
        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            var eventItem = _eventRepository.GetFirstOrDefault(e => e.Id == eventId);
            if (eventItem == null)
            {
                TempData["error"] = "Event does not exist.";
                return RedirectToAction("Index", "Event");
            }

            if (eventItem.IsSponsored)
            {
                TempData["error"] = "This event already has a sponsored ad.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var model = new SponsoredAdViewModel
            {
                EventId = eventId,
                Title = eventItem.Name,
                Description = eventItem.Description,
                Price = 5000.00m, // Default price
                Duration = 30 // Default duration in days
            };

            return View(model);
        }

        // POST: SponsoredAd/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SponsoredAdViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var eventItem = _eventRepository.GetFirstOrDefault(e => e.Id == model.EventId);
            if (eventItem == null)
            {
                TempData["error"] = "Event does not exist.";
                return RedirectToAction("Index", "Event");
            }

            if (eventItem.IsSponsored)
            {
                ModelState.AddModelError("", "This event already has a sponsored ad.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction("Index", "Event");
            }

            var sponsoredAd = new SponsoredAd
            {
                Title = model.Title,
                Description = model.Description,
                Duration = model.Duration,
                Price = model.Price,
                IsPaid = false,
                UserId = user.Id,
                EventId = model.EventId,
                CreatedAt = DateTime.Now
            };

            _sponsoredAdRepository.create(sponsoredAd);
            _sponsoredAdRepository.commit();

            // إعدادات الجلسة على Stripe
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(model.Price * 100), // تحويل السعر إلى سنتات
                    Currency = "egp",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = model.Title,
                        Description = model.Description,
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/SponsoredAd/Success?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/SponsoredAd/Cancel",
                ClientReferenceId = sponsoredAd.Id.ToString() // ربط الإعلان بجلسة الدفع
            };

            var service = new SessionService();
            Session session = service.Create(options);

            // تخزين PaymentIntentId في الإعلان المدعوم
            sponsoredAd.PaymentIntentId = session.PaymentIntentId;

            // حفظ التغييرات في قاعدة البيانات
            _sponsoredAdRepository.Edit(sponsoredAd);
            _sponsoredAdRepository.commit();

            // إعادة التوجيه إلى Stripe Checkout
            return Redirect(session.Url);
        }



        // GET: SponsoredAd/Success
        [HttpGet]
        public async Task<IActionResult> Success(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                TempData["error"] = "Invalid payment session.";
                return RedirectToAction("Index", "Event");
            }

            var service = new SessionService();
            Session session = service.Get(sessionId);

            if (session.PaymentStatus == "paid")
            {
                var sponsoredAd = _sponsoredAdRepository.GetFirstOrDefault(sa => sa.Id.ToString() == session.ClientReferenceId);

                if (sponsoredAd != null && !sponsoredAd.IsPaid)
                {
                    sponsoredAd.IsPaid = true;
                    _sponsoredAdRepository.Edit(sponsoredAd);
                    _sponsoredAdRepository.commit();

                    var eventItem = _eventRepository.GetFirstOrDefault(e => e.Id == sponsoredAd.EventId);
                    if (eventItem != null)
                    {
                        eventItem.IsSponsored = true;
                        eventItem.Available = true; 
                        eventItem.SponsoredAdId = sponsoredAd.Id; 
                        _eventRepository.Edit(eventItem);
                        _eventRepository.commit();
                    }

                    var user = await _userManager.FindByIdAsync(sponsoredAd.UserId);
                    if (user != null)
                    {
                        await _emailSender.SendEmailAsync(user.Email, "Sponsored Ad Confirmation", $"Your sponsored ad for the event '{eventItem.Name}' has been activated.");
                    }

                    TempData["success"] = "Payment successful. Sponsored ad activated.";
                    return View();
                }
            }

            TempData["error"] = "Payment processing failed.";
            return View();
        }


        // GET: SponsoredAd/Cancel
        [HttpGet]
        public IActionResult Cancel(int eventId)
        {
            TempData["error"] = "Payment was canceled.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessRefund(int adId)
        {
            var sponsoredAd = _sponsoredAdRepository.GetFirstOrDefault(sa => sa.Id == adId);
            if (sponsoredAd != null && sponsoredAd.IsPaid)
            {
                // التهيئة لخدمة الاسترجاع في Stripe
                var refundService = new RefundService();
                var chargeService = new ChargeService();

                try
                {
                    var charges = await chargeService.ListAsync(new ChargeListOptions
                    {

                    });

                    // التأكد من وجود شحنات
                    if (charges.Data.Count == 0)
                    {
                        TempData["error"] = "No charges found for this ad.";
                        return RedirectToAction("Index", "SponsoredAdReport");
                    }

                    // الحصول على ChargeId من أول شحنة
                    var chargeId = charges.Data[0].Id;

                    // إنشاء طلب استرجاع باستخدام ChargeId
                    var refundOptions = new RefundCreateOptions
                    {
                        Charge = chargeId
                    };

                    var refund = await refundService.CreateAsync(refundOptions);

                    if (refund.Status == "succeeded")
                    {
                        // تحديث حالة الإعلان الممول إلى "لم يتم الدفع"
                        sponsoredAd.IsPaid = false;
                        _sponsoredAdRepository.Edit(sponsoredAd);
                        _sponsoredAdRepository.commit();

                        // تحديث الحدث المرتبط بالإعلان الممول
                        var eventItem = _eventRepository.GetFirstOrDefault(e => e.Id == sponsoredAd.EventId);
                        if (eventItem != null)
                        {
                            eventItem.IsSponsored = false;
                            eventItem.SponsoredAdId = null;
                            _eventRepository.Edit(eventItem);
                            _eventRepository.commit();
                        }

                        var user = await _userManager.GetUserAsync(User);
                        if (user != null)
                        {
                            // استرجاع الحدث المرتبط بالإعلان الممول
                            var eventItem1 = _eventRepository.GetFirstOrDefault(e => e.Id == sponsoredAd.EventId);

                            if (eventItem1 != null)
                            {
                                // استرجاع مالك الحدث باستخدام UserId
                                var eventOwner = await _userManager.FindByIdAsync(eventItem1.UserId);

                                if (eventOwner != null)
                                {
                                    var emailSubject = "Refund Processed for Sponsored Ad";
                                    var emailBody = $@"
            <h3>Refund Confirmation</h3>
            <p>Dear {eventOwner.UserName},</p>
            <p>We are writing to inform you that the payment for the sponsored ad for your event '{eventItem1.Name}' has been successfully refunded.</p>
            <p>Refund Amount: EGP {sponsoredAd.Price}</p>
            <p>If you have any further questions, please feel free to contact us.</p>
            <p>Best regards,</p>
            <p>Your Company Name</p>
            ";

                                    // إرسال الإيميل لصاحب الحدث
                                    await _emailSender.SendEmailAsync(eventOwner.Email, emailSubject, emailBody);
                                }
                                TempData["success"] = "Payment has been refunded and sponsored ad deactivated.";
                                return RedirectToAction("Index", "SponsoredAdReport");
                            }
                            else
                            {
                                TempData["error"] = "Refund failed. Please try again.";
                                return RedirectToAction("Index", "SponsoredAdReport");
                            }
                        }
                    }
                }
                catch (StripeException stripeEx)
                {
                    // معالجة استثناءات Stripe
                    TempData["error"] = $"Stripe Error: {stripeEx.Message}";
                    return RedirectToAction("Index", "SponsoredAdReport");
                }
                catch (Exception ex)
                {
                    // معالجة الاستثناءات العامة
                    TempData["error"] = $"An error occurred: {ex.Message}";
                    return RedirectToAction("Index", "SponsoredAdReport");
                }
            }

            TempData["error"] = "Refund failed. Payment or ad not found.";
            return RedirectToAction("Index", "SponsoredAdReport");
        }

    }

}

