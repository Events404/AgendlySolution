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
                    LockoutEnd = user.LockoutEnd,
                    Roles = new List<string>()
                }).ToListAsync();

            foreach (var user in users)
            {
                user.Roles = await userManager.GetRolesAsync(await userManager.FindByIdAsync(user.Id));
            }

            return View(users);
        }


        public async Task<IActionResult> Index1()
        {
            var users = await userManager.Users.ToListAsync();
            var blockedUsersCount = users.Count(user => user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow);
            var activeUsersCount = users.Count(user => user.LockoutEnd == null || user.LockoutEnd <= DateTime.UtcNow);

            ViewBag.BlockedUsersCount = blockedUsersCount;
            ViewBag.ActiveUsersCount = activeUsersCount;

            var userCount = await userManager.Users.CountAsync(); 

            ViewBag.UserCount = userCount;

            return View();
        }
       


        //[HttpGet]
        //public async Task<IActionResult> ManageRole(string userId)
        //{
        //    var user = await userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return NotFound();
        //    var role = await roleManager.Roles.ToListAsync();

        //    var ViewModel = new UserViewModel
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Roles = role.Select(role => new RoleViewModel
        //        {

        //            Id = role.Id,
        //            Name = role.Name,
        //            NormalizedName = role.NormalizedName,
        //            IsSelected = userManager.IsInRoleAsync(user, role.Name).Result


        //        }).ToList(),
        //    };

        //    return View(ViewModel);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ManageRole(UserViewModel model)
        //{
        //    var user = await userManager.FindByIdAsync(model.Id);
        //    if (user == null)
        //        return NotFound();
        //    var UserRole = await userManager.GetRolesAsync(user);
        //    foreach (var role in model.Roles)
        //    {
        //        if (UserRole.Any(r => r == role.Name && !role.IsSelected))
        //        {
        //            await userManager.RemoveFromRoleAsync(user, role.Name);
        //        }

        //        if (!UserRole.Any(r => r == role.Name && role.IsSelected))
        //        {
        //            await userManager.AddToRoleAsync(user, role.Name);
        //        }

        //    }
        //    return RedirectToAction(nameof(Index));

        //}

        [HttpGet]
        public async Task<IActionResult> ManageRole(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var allRoles = await roleManager.Roles.ToListAsync();
            var userRoles = await userManager.GetRolesAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = allRoles.Select(role => new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    NormalizedName = role.NormalizedName,
                    IsSelected = userRoles.Contains(role.Name)
                }).ToList(),
            };

            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRole(UserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            var currentRoles = await userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.Name).ToList();

            // الأدوار التي يجب إضافتها وإزالتها
            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

            // إزالة الأدوار
            if (rolesToRemove.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove roles.");
                    return View(model);
                }
            }

            // إضافة الأدوار
            if (rolesToAdd.Any())
            {
                var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add roles.");
                    return View(model);
                }
            }

            // إذا كان كل شيء صحيحًا، أعد التوجيه إلى الصفحة الرئيسية أو عرض التأكيد
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = DateTime.Now.AddDays(30); 
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "User has been blocked.";
            }
            else
            {
                TempData["Message"] = "Error while blocking user.";
            }

            return RedirectToAction(nameof(Index)); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = null;

            var result = await userManager.UpdateAsync(user);  
            if (result.Succeeded)
            {
                TempData["Message"] = "User has been unblocked.";
            }
            else
            {
                TempData["Message"] = "Error while unblocking user.";
            }

            return RedirectToAction(nameof(Index)); 
        }





    }
}
