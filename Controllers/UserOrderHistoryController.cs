using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    public class UserOrderHistoryController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public UserOrderHistoryController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            var userOrderNumbers = await _db.UserCheckOutInputs
                .Where(i => i.ApplicationUserId == user.Id)
                .Select(i => i.OrderNumber)
                .ToListAsync();

            IList<UserCheckOutInput> userCheckOutInput = await _db.UserCheckOutInputs
                .Where(i => i.ApplicationUserId == user.Id)
                .ToListAsync();

            IList<InventoryItem> inventoryItems = await _db.InventoryItems
                .Where(i => userOrderNumbers.Contains((int)i.OrderNumber))
                .Include(i => i.BoardGame)
                .OrderBy(i => i.OrderedDate)
                .ThenBy(i => i.OrderNumber)
                .ToListAsync();

            UserOrderHistoryViewModel userOrderHistoryViewModel = new UserOrderHistoryViewModel()
            {
                InventoryItems = inventoryItems,
                UserCheckOutInput = userCheckOutInput
            };

            return View(userOrderHistoryViewModel);
        }
    }
}
