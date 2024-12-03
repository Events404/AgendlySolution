// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace Agendly.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "First Name is required.")]
            [StringLength(16, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 16 characters.")]
            [Display(Name = "First Name")]

            public String FirstName { get; set; }
            [Required(ErrorMessage = "List Name is required.")]
            [StringLength(16, MinimumLength = 2, ErrorMessage = "List Name must be between 2 and 16 characters.")]
            [Display(Name = "List Name")]
            public String LastName { get; set; }
            [Required(ErrorMessage = "Country is required.")]
            [StringLength(16, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 16 characters.")]
            [Display(Name = "Country")]
            public String Country { get; set; }
            [Required(ErrorMessage = "City is required.")]
            [StringLength(16, MinimumLength = 2, ErrorMessage = "City must be between 2 and 16 characters.")]
            [Display(Name = "City")]
            public String City { get; set; }
            public byte[] ProfilePicture { get; set; }

            [Display(Name = "Phone number")]
            [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "The phone number must start with 010, 011, or 012 or 015 and must be 11 digits long.")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                Country =user.Country,
                ProfilePicture = user.ProfilePicture,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var FirstName = user.FirstName;
            var LastName = user.LastName;
            var City = user.City;
            var Country = user.Country;
            var ProfilePicture = user.ProfilePicture;
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    using (var DataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(DataStream);
                        user.ProfilePicture = DataStream.ToArray();
                        await _userManager.UpdateAsync(user);

                        TempData["StatusMessage"] = "Your profile updated successfully!";

                    }
                }
                else
                {

                    TempData["StatusMessage"] = "The uploaded file is not an image. Please upload a file in (JPG, PNG, GIF, BMP) format.";

                    return RedirectToAction("Index", "Identity", "Account", "Manage");

                }

                return RedirectToAction("Index","Identity", "Account", "Manage");

            }


            if (Input.FirstName != FirstName)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);
            }
            if (Input.LastName != LastName)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);
            }
            if (Input.City != City)
            {
                user.City = Input.City;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Country != Country)
            {
                user.Country = Input.Country;
                await _userManager.UpdateAsync(user);
            }
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
