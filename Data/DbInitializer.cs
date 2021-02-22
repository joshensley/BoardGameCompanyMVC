using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Data
{
   

    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async void Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_db.Roles.Any(r => r.Name == SD.AdminEndUser)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName="joshensley@gmail.com",
                Email="joshensley@gmail.com",
                Name="Josh Hensley",
                EmailConfirmed=true,
                PhoneNumber="1112223333"

            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "joshensley@gmail.com");

            _userManager.AddToRoleAsync(user, SD.AdminEndUser).GetAwaiter().GetResult();
        }
    }
}
