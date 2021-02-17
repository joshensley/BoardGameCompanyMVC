using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    public class UserOrderConfirmationController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public UserOrderConfirmationController(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string orderNumber)
        {

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            IQueryable<UserOrder> userOrderIQ = _db.UserOrders
                .Where(i => i.ApplicationUserId == user.Id)
                .Where(i => i.OrderNumber.ToString() == orderNumber)
                .Include(i => i.BoardGame)
                    .ThenInclude(b => b.Brand);

            IList<UserOrder> UserOrder = await userOrderIQ.ToListAsync();

            if (UserOrder.Count() == 0)
            {
                return NotFound();
            }

            UserCheckOutInput UserCheckOutInput = await _db.UserCheckOutInputs
                .FirstOrDefaultAsync(i => i.OrderNumber.ToString() == orderNumber);

            UserOrderConfirmationViewModel UserOrderConfirmationViewModel = new UserOrderConfirmationViewModel()
            {
                UserOrder = UserOrder,
                UserCheckOutInput = UserCheckOutInput
            };

            return View(UserOrderConfirmationViewModel);
        }
    }
}
