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
    public class StoreController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public StoreController(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager
            )
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            BoardGame BoardGame = await _db.BoardGames
                .Include(b => b.InventoryItems.Where(i => i.InStock == true))
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(b => b.ID == id);

            IQueryable<UserReviewGroup> UserReviewGroupsIQ = _db.UserBoardGameReviews
                .Where(u => u.BoardGameID == id)
                .Join(_db.ApplicationUser,
                    userReview => userReview.UserIDReview,
                    applicationUser => applicationUser.Id,
                    (userReview, applicationUser) => new UserReviewGroup() 
                    {
                        Id = applicationUser.Id,
                        Name = applicationUser.Name,
                        UserAvatarFilePath = applicationUser.UserAvatarFilePath,
                        BoardGameID = userReview.BoardGameID,
                        UserReview = userReview.UserReview,
                        ReviewDate = userReview.UserReviewDate,
                    });

            IEnumerable<UserReviewGroup> UserReviewGroups = await UserReviewGroupsIQ.ToListAsync();

            BoardGameStoreViewModel BoardGameStoreViewModel;

            if (User.Identity.IsAuthenticated) {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                var userReview = await _db.UserBoardGameReviews
                    .FirstOrDefaultAsync(
                        u => u.UserIDReview == user.Id && u.BoardGameID == id);

                var userHasReview = userReview != null ? true : false;

                BoardGameStoreViewModel = new BoardGameStoreViewModel()
                {
                    BoardGame = BoardGame,
                    UserReviewGroups = UserReviewGroups,
                    UserId = user.Id,
                    UserHasReview = userHasReview
                };

            }
            else
            {
                BoardGameStoreViewModel = new BoardGameStoreViewModel()
                {
                    BoardGame = BoardGame,
                    UserReviewGroups = UserReviewGroups
                };
            }
            
            return View(BoardGameStoreViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(
            [Bind("BoardGameID", "Quantity", "ApplicationUserId")] UserCart UserCart)
        {

            // Redirects to login page is user is not authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // If the item is already in the user cart, user will be redirected to
            // user cart without adding the item to the cart again.
            var userCartDb = await _db.UserCart
                .Where(u => u.ApplicationUserId == UserCart.ApplicationUserId)
                .Where(u => u.BoardGameID == UserCart.BoardGameID)
                .FirstOrDefaultAsync();

            if (userCartDb != null)
            {
                return RedirectToAction("Index", "UserCart", new { area = "" });
            }

            // Adds the item to the user cart then redirects to cart
            if (ModelState.IsValid)
            {
                var emptyUserCart = new UserCart();

                await TryUpdateModelAsync<UserCart>(
                    emptyUserCart,
                    "usercart",
                    u => u.BoardGameID,
                    u => u.Quantity,
                    u => u.ApplicationUserId);
                {
                    _db.UserCart.Add(emptyUserCart);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "UserCart", new { area = "" });
                }
            }

            return RedirectToAction("Index", "UserCart", new { area = "" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserReview", "UserReviewDate", "UserIDReview", "BoardGameID")] UserBoardGameReview UserBoardGameReview,
            int? id)
        {

            if (ModelState.IsValid)
            {
                var emptyUserReview = new UserBoardGameReview();

                await TryUpdateModelAsync<UserBoardGameReview>(
                    emptyUserReview,
                    "userboardgamereview",
                    u => u.UserReview,
                    u => u.UserReviewDate,
                    u => u.BoardGameID,
                    u => u.UserIDReview);
                {
                    _db.UserBoardGameReviews.Add(emptyUserReview);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = id });
                }
            }

            return View(UserBoardGameReview);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("UserReview", "UserReviewDate", "UserIDReview", "BoardGameID")] UserBoardGameReview UserBoardGameReview, 
            int id)
        {
           
            var userReviewToUpdate = await _db.UserBoardGameReviews.FindAsync(id);

            if (userReviewToUpdate == null)
            {
                return null;
            }

            await TryUpdateModelAsync<UserBoardGameReview>(
            userReviewToUpdate,
            "",
            u => u.UserReview,
            u => u.UserReviewDate,
            u => u.UserIDReview,
            u => u.BoardGameID);
            {
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", new { id = userReviewToUpdate.BoardGameID });
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            var userReview = await _db.UserBoardGameReviews
                .Where(u => u.UserIDReview == user.Id)
                .FirstOrDefaultAsync(u => u.BoardGameID == id);

            if (userReview == null)
            {
                return NotFound();
            }

            return View(userReview);
        } 

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? idDelete)
        {
            if (idDelete == null)
            {
                return NotFound();
            }

            var deleteReview = await _db.UserBoardGameReviews.FindAsync(idDelete);

            if (deleteReview == null)
            {
                return NotFound();
            }

            _db.UserBoardGameReviews.Remove(deleteReview);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = deleteReview.BoardGameID });
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            var userReview = await _db.UserBoardGameReviews
                .Where(u => u.UserIDReview == user.Id)
                .FirstOrDefaultAsync(u => u.BoardGameID == id);

            if (userReview == null)
            {
                return NotFound();
            }

            return View(userReview);
        }

    }
}
