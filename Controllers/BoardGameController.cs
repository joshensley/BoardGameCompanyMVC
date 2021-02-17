using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.Utility;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class BoardGameController : Controller
    {

        public readonly ApplicationDbContext _db;
        public readonly IWebHostEnvironment _environment;

        public BoardGameController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public async Task<IActionResult> Index(
            string sortOrder, 
            string searchString,
            string currentFilter,
            int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder == null ? "" : sortOrder;
            ViewData["TitleParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PlayerNumberParm"] = sortOrder == "playerNumber_desc" ? "playerNumber_asc" : "playerNumber_desc";
            ViewData["ReleaseDateParm"] = sortOrder == "releaseDate_desc" ? "releaseDate_asc" : "releaseDate_desc";
            ViewData["PriceParm"] = sortOrder == "price_desc" ? "price_asc" : "price_desc";
            ViewData["InStockParm"] = sortOrder == "inStock_desc" ? "inStock_asc" : "inStock_desc";

            if(searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            IQueryable<BoardGame> boardGame = _db.BoardGames
                .Include(b => b.InventoryItems.Where(i => i.InStock == true));

            if (!String.IsNullOrEmpty(searchString))
            {
                boardGame = boardGame.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    boardGame = boardGame.OrderByDescending(s => s.Title);
                    break;
                case "playerNumber_asc":
                    boardGame = boardGame.OrderBy(s => s.PlayerNumber);
                    break;
                case "playerNumber_desc":
                    boardGame = boardGame.OrderByDescending(s => s.PlayerNumber);
                    break;
                case "releaseDate_asc":
                    boardGame = boardGame.OrderBy(s => s.ReleaseDate);
                    break;
                case "releaseDate_desc":
                    boardGame = boardGame.OrderByDescending(s => s.ReleaseDate);
                    break;
                case "price_asc":
                    boardGame = boardGame.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    boardGame = boardGame.OrderByDescending(s => s.Price);
                    break;
                case "inStock_asc":
                    boardGame = boardGame.OrderBy(s => s.InventoryItems.Count());
                    break;
                case "inStock_desc":
                    boardGame = boardGame.OrderByDescending(s => s.InventoryItems.Count());
                    break;
                default:
                    boardGame = boardGame.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<BoardGame>.CreateAsync(boardGame.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UPC", "Title", "Description", 
            "PlayerNumber", "ReleaseDate", "Price", 
            "ImageFilePath", "FormFileUpload", "BrandID")] BoardGame boardGame)
        {
            if(ModelState.IsValid)
            {
                var filepath = "/images/boardgames/defaultBoardGame/Lead-Gray.jpg";
                if(boardGame.FormFileUpload != null)
                {
                    var rand = new Random();
                    var randNum = rand.Next(100000, 1000000);
                    var fileName = randNum.ToString() + "_" + boardGame.FormFileUpload.FileName;
                    filepath = $"/images/boardgames/{fileName}";
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/boardgames/", fileName);

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await boardGame.FormFileUpload.CopyToAsync(fileStream);
                    }

                }

                var emptyBoardGame = new BoardGame();

                if(await TryUpdateModelAsync<BoardGame>(
                    emptyBoardGame,
                    "boardgame",
                    b => b.UPC,
                    b => b.Title,
                    b => b.Description,
                    b => b.PlayerNumber,
                    b => b.ReleaseDate,
                    b => b.Price,
                    b => b.ImageFilePath,
                    b => b.BrandID))
                {
                    emptyBoardGame.ImageFilePath = filepath;
                    _db.Add(emptyBoardGame);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(boardGame);
        }

        public async Task<IActionResult> Create()
        {

            BoardGame boardGame = new BoardGame()
            {
                ReleaseDate = DateTime.UtcNow.Date
            };

            IEnumerable<Brand> brands = await _db.Brands
                .OrderBy(b => b.BrandName)
                .ToListAsync();

            BoardGameDetailsViewModel boardGameDetailsViewModel = new BoardGameDetailsViewModel()
            {
                BoardGame = boardGame,
                Brands = brands
            };

            return View(boardGameDetailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
             [Bind("UPC", "Title", "Description",
             "PlayerNumber", "ReleaseDate", "Price",
             "ImageFilePath", "FormFileUpload", "BrandID")] BoardGame boardGame)
        {
            var boardGameToUpdate = await _db.BoardGames.FindAsync(id);

            if (boardGameToUpdate == null)
            {
                return NotFound();
            }

            var filepath = boardGame.ImageFilePath;
            var oldFilePath = boardGame.ImageFilePath;

            if (ModelState.IsValid)
            {
                if (boardGame.FormFileUpload != null)
                {
                    var rand = new Random();
                    var randNum = rand.Next(100000, 1000000);
                    var fileName = randNum.ToString() + "_" + boardGame.FormFileUpload.FileName;
                    filepath = $"/images/boardgames/{fileName}";
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/boardgames/", fileName);

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await boardGame.FormFileUpload.CopyToAsync(fileStream);
                    }

                    // Will only delete if the path exists and if the path is not pointed to the default avatar image
                    var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{boardGameToUpdate.ImageFilePath}");
                    if (System.IO.File.Exists(deleteFile) && (oldFilePath != "/images/boardgames/defaultBoardGame/Lead-Gray.jpg"))
                    {
                        System.IO.File.Delete(deleteFile);
                    }
                }

                if (await TryUpdateModelAsync<BoardGame>(
                        boardGameToUpdate,
                        "boardgame",
                        b => b.UPC,
                        b => b.Title,
                        b => b.Description,
                        b => b.PlayerNumber,
                        b => b.ReleaseDate,
                        b => b.Price,
                        b => b.ImageFilePath,
                        b => b.BrandID))
                    {
                        if (boardGame.FormFileUpload != null)
                        {
                            boardGameToUpdate.ImageFilePath = filepath;
                        }
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
   
                }

            IEnumerable<Brand> brands = await _db.Brands.ToListAsync();

            BoardGameDetailsViewModel boardGameDetailsViewModel = new BoardGameDetailsViewModel()
            {
                Brands = brands,
                BoardGame = boardGame
            };

            return View(boardGameDetailsViewModel);
        }
            

        public async Task<IActionResult> Edit(int? id)
        {

            BoardGame boardGame = await _db.BoardGames.FindAsync(id);

            if(boardGame == null)
            {
                return NotFound();
            }

            IEnumerable<Brand> brands = await _db.Brands
                .OrderBy(b => b.BrandName)
                .ToListAsync();

            BoardGameDetailsViewModel boardGameDetailsViewModel = new BoardGameDetailsViewModel()
            {
                BoardGame = boardGame,
                Brands = brands
            };

            return View(boardGameDetailsViewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var boardGame = await _db.BoardGames.FirstOrDefaultAsync(b => b.ID == id);

            if(boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var boardGame = await _db.BoardGames.FindAsync(id);

            if(boardGame == null)
            {
                return NotFound();
            }

            _db.BoardGames.Remove(boardGame);

            var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{boardGame.ImageFilePath}");

            if (System.IO.File.Exists(deleteFile) && (boardGame.ImageFilePath != "/images/boardgames/defaultBoardGame/Lead-Gray.jpg"))
            {
                System.IO.File.Delete(deleteFile);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var boardGame = await _db.BoardGames.FirstOrDefaultAsync(b => b.ID == id);
            
            if(boardGame == null)
            {
                return NotFound();
            }

            return View(boardGame);
        }

    }
}
