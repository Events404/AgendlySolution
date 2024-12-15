using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace Agendly.Controllers
{

    public class CategorieController : Controller
    {
        private readonly CategoryIRepository categoryRepository;

        public CategorieController(CategoryIRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [Authorize(Roles = $"{SD.AdminRoles}")]
        public IActionResult Index()
        {
            var categories = categoryRepository.GatAll();
            return View(categories);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRoles}")]
        public IActionResult Create(Categorie categorie)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.create(categorie);
                categoryRepository.commit();
                TempData["success"] = "Category added successfully!";
                return RedirectToAction(nameof(Index));  
            }
            return RedirectToAction(nameof(Index)); 
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRoles}")]
        public IActionResult Edit(Categorie categorie)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Edit(categorie);
                categoryRepository.commit();
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));  
            }
            return RedirectToAction(nameof(Index)); 
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRoles}")]
        public IActionResult Delete(int CategorieId)
        {
            var categorie = categoryRepository.GetById(CategorieId);
            if (categorie != null)
            {
                categoryRepository.Delete(categorie);
                categoryRepository.commit();
                TempData["success"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Index));  
        }
    }

}
