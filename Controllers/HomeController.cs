using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    public class HomeController : Controller
    {

        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
            string sortOrder,
            string searchString,
            string currentFilter,
            int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder == null ? "" : sortOrder;
            ViewData["TitleParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

            if(searchString != null)
            {
                pageNumber = 1;
            } 
            else
            {
                searchString = currentFilter;
                ViewData["CurrentPageNumber"] = pageNumber;
            }

            ViewData["CurrentFilter"] = searchString;

            // Gets board games from database
            IQueryable<BoardGame> boardGames = _db.BoardGames
                .Include(b => b.InventoryItems.Where(i => i.InStock == true));

            if (!String.IsNullOrEmpty(searchString))
            {
                boardGames = boardGames.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    boardGames = boardGames.OrderByDescending(s => s.Title);
                    break;
                default:
                    boardGames = boardGames.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 3;
            PaginatedList<BoardGame> BoardGames = await PaginatedList<BoardGame>.CreateAsync(boardGames.AsNoTracking(), pageNumber ?? 1, pageSize);

            // Headlines
            IList<Headlines> Headlines = await _db.Headlines
                .Where(i => i.IsActive == true)
                .ToListAsync();


            // Returns view base on whether user is authenticated
            HomeViewModel HomeViewModel;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string userId = user.Id;

                HomeViewModel = new HomeViewModel()
                {
                    BoardGames = BoardGames,
                    Headlines = Headlines,
                    UserId = userId
                };

                return View(HomeViewModel);
            }

            HomeViewModel = new HomeViewModel()
            {
                BoardGames = BoardGames,
                Headlines = Headlines
            };

            return View(HomeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(
            [Bind("BoardGameID", "Quantity", "ApplicationUserId")] UserCart UserCart)
        {
            // Redirects to login page if user is not authenticated
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

            return RedirectToAction("Index");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
