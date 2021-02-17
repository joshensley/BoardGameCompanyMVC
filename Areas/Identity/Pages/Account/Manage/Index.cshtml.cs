using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoardGameCompanyMVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCompanyMVC.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string UserName { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string FilePath { get; set; }
            public IFormFile Upload { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userManager = await _userManager.GetUserAsync(User);

            Input = new InputModel
            {
                UserName = userManager.UserName,
                PhoneNumber = userManager.PhoneNumber,
                Name = userManager.Name,
                Address = userManager.Address,
                City = userManager.City,
                State = userManager.State,
                PostalCode = userManager.PostalCode,
                FilePath = userManager.UserAvatarFilePath
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
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var filepath = Input.FilePath;

            if (Input.Upload != null)
            {
                var oldFilePath = Input.FilePath;
                var rand = new Random();
                var randNum = rand.Next(100000, 1000000);
                var fileName = randNum.ToString() + "_" + Input.Upload.FileName; 
                filepath = $"/images/userAvatar/{fileName}";

                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/userAvatar/", fileName);
                Input.FilePath = $"/images/userAvatar/{Input.Upload.FileName}";

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Input.Upload.CopyToAsync(fileStream);
                }

                // Will only delete if the path exists and if the path is not pointed to the default avatar image
                var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{user.UserAvatarFilePath}");
                if (System.IO.File.Exists(deleteFile) && (oldFilePath != "/images/userAvatar/defaultAvatar/defaultAvatar.png"))
                {
                    System.IO.File.Delete(deleteFile);
                }   
            }

            if (await TryUpdateModelAsync<ApplicationUser>(
                user,
                "input",
                a => a.PhoneNumber,
                a => a.Name,
                a => a.Address,
                a => a.City,
                a => a.PostalCode,
                a => a.UserAvatarFilePath))
            {
                if (Input.Upload != null)
                {
                    user.UserAvatarFilePath = filepath;
                }

                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
