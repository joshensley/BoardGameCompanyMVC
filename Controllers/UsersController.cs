using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.Utility;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class UsersController : Controller
    {
        public readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.ApplicationUser.ToListAsync());
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("Id","Name", "PhoneNumber", "Address", "City", "State", "PostalCode")] ApplicationUser applicationUser)
        {
            var userToUpdate = await _db.ApplicationUser.FindAsync(id);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await TryUpdateModelAsync<ApplicationUser>(
                    userToUpdate,
                    "",
                    u => u.Id,
                    u => u.Name,
                    u => u.PhoneNumber,
                    u => u.Address,
                    u => u.City,
                    u => u.State,
                    u => u.PostalCode
                    ))
                {
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(userToUpdate);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FirstOrDefaultAsync(a => a.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _db.ApplicationUser.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FirstOrDefaultAsync(a => a.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


    }
}
