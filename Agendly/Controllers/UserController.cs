using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;
using Utility.ViewModel;

namespace Agendly.Controllers
{
    [Authorize(Roles = SD.AdminRoles)]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // أولاً، جلب جميع المستخدمين
            var users = await userManager.Users
                .Select(user => new ViewModelUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    City = user.City,
                    Country = user.Country,

                    // هنا سنترك الأدوار فارغة مؤقتًا
                    Roles = new List<string>()
                }).ToListAsync();

            // الآن، جلب الأدوار لكل مستخدم بشكل منفصل باستخدام await
            foreach (var user in users)
            {
                user.Roles = await userManager.GetRolesAsync(await userManager.FindByIdAsync(user.Id));
            }

            return View(users);
        }


        public async Task<IActionResult> Index1()
        {
            var userCount = await userManager.Users.CountAsync(); // عدد المستخدمين

            // تخزين البيانات في ViewBag
            ViewBag.UserCount = userCount;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ManageRole(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            var role = await roleManager.Roles.ToListAsync();

            var ViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = role.Select(role => new RoleViewModel
                {

                    Id = role.Id,
                    Name = role.Name,
                    IsSelected = userManager.IsInRoleAsync(user,role.Name).Result


                }).ToList(),
            };

            return View(ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRole(UserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                // التأكد من أن role.Name ليس null أو فارغ
                if (string.IsNullOrEmpty(role.Name))
                {
                    ModelState.AddModelError("", "Role name cannot be null or empty.");
                    return View(model);
                }

                if (userRoles.Any(e => e == role.Name) && !role.IsSelected)
                    await userManager.RemoveFromRoleAsync(user, role.Name);

                if (!userRoles.Any(r => r == role.Name) && role.IsSelected)
                    await userManager.AddToRoleAsync(user, role.Name);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
