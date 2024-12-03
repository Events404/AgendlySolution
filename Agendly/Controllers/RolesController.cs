using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Utility;
using Utility.ViewModel;

namespace Agendly.Controllers
{
    [Authorize(Roles = SD.AdminRoles)]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ViewModelRoles roles)
        {
            if (!ModelState.IsValid)
              return View("Index" , await roleManager.Roles.ToListAsync());
            
            if (await roleManager.RoleExistsAsync(roles.Name))
            {
                ModelState.AddModelError("Name", "Role Is Exists!");
                return View("Index", await roleManager.Roles.ToListAsync());

            }
            await roleManager.CreateAsync(new IdentityRole { Name = roles.Name.Trim() });
            return RedirectToAction(nameof(Index));
        }
    }
}
