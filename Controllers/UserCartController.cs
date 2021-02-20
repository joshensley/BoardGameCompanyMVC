using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,Customer")]
    public class UserCartController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public UserCartController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index(
            int? boardGameId,
            List<int>? boardGameIdList,
            int? currentStock, 
            bool? changeQuantityError = false)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            string userId = user.Id.ToString();

            IList<UserCart> UserCart = await _db.UserCart
                .Where(u => u.ApplicationUserId == userId)
                .Include(u => u.ApplicationUser)
                .Include(u => u.BoardGame)
                    .ThenInclude(u => u.Brand)
                .ToListAsync();


            IList<BoardGame> boardGames = new List<BoardGame>();
            if (boardGameIdList.Count > 0)
            {
                foreach (var item in boardGameIdList)
                {
                    BoardGame boardGame = await _db.BoardGames
                        .Include(i => i.InventoryItems.Where(i => i.InStock == true))
                        .FirstOrDefaultAsync(i => i.ID == item);

                    boardGames.Add(boardGame);
                }
            }


            string ErrorMessage = "";
            int? ErrorBoardGameID = 0;
            if (changeQuantityError == true)
            {
                ErrorMessage = $"Only {currentStock} in stock";
                ErrorBoardGameID = boardGameId;
            }

            UserCartViewModel UserCartViewModel = new UserCartViewModel()
            {
                UserCart = UserCart,
                ErrorMessage = ErrorMessage,
                ErrorBoardGameID = ErrorBoardGameID,
                BoardGames = boardGames
            };

            return View(UserCartViewModel);
        }

        public async Task<IActionResult> CheckOut()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var userCart = await _db.UserCart
                .Where(u => u.ApplicationUserId == user.Id)
                .ToListAsync();

            List<int> boardGameIdList = new List<int>();
            foreach(var item in userCart)
            {
                var inventoryItemCount = _db.InventoryItems
                    .Where(i => i.BoardGameID == item.BoardGameID)
                    .Where(i => i.InStock == true)
                    .Count();

                if (item.Quantity > inventoryItemCount)
                {
                    boardGameIdList.Add(item.BoardGameID);
                }
            }

            if (boardGameIdList.Count() > 0)
            {
                return RedirectToAction("Index", new { boardGameIdList = boardGameIdList });
            }

            return RedirectToAction("Index", "UserCheckOut");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncreaseQuantity(
            [Bind("Quantity")] UserCart UserCart,
            int? boardGameID,
            int? lineItemId)
        {
            if (lineItemId == null || boardGameID == null)
            {
                return NotFound();
            }

            var userCartToUpdate = await _db.UserCart.FindAsync(lineItemId);

            if (userCartToUpdate == null)
            {
                return NotFound();
            }

            int currentStock = _db.InventoryItems
                .Where(i => i.BoardGameID == boardGameID)
                .Where(i => i.InStock == true)
                .Count();

            await TryUpdateModelAsync<UserCart>(
                userCartToUpdate,
                "item",
                u => u.Quantity);
            {    
                if (userCartToUpdate.Quantity >= currentStock)
                {
                    return RedirectToAction("Index", new
                    {
                        changeQuantityError = true,
                        currentStock,
                        boardGameID
                    });
                }

                userCartToUpdate.Quantity += 1;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecreaseQuantity(
            [Bind("Quantity")] UserCart UserCart,
            int? boardGameID,
            int? lineItemId)
        {
            if (lineItemId == null || boardGameID == null)
            {
                return NotFound();
            }

            var userCartToUpdate = await _db.UserCart.FindAsync(lineItemId);

            if (userCartToUpdate == null)
            {
                return NotFound();
            }

            await TryUpdateModelAsync<UserCart>(
                userCartToUpdate,
                "item",
                u => u.Quantity);
            {
                if (userCartToUpdate.Quantity <= 1)
                {
                    return RedirectToAction("Index");
                }

                userCartToUpdate.Quantity -= 1;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            var deleteLineItem = await _db.UserCart
                .Where(u => u.ApplicationUserId == user.Id)
                .FirstOrDefaultAsync(u => u.ID == id);

            if (deleteLineItem == null )
            {
                return NotFound();
            }

            _db.UserCart.Remove(deleteLineItem);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }


    }
}
