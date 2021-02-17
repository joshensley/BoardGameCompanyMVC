using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.Utility;
using BoardGameCompanyMVC.ViewModels;
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
    public class InventoryItemController : Controller
    {

        public readonly ApplicationDbContext _db;

        public InventoryItemController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string sortOrder, 
            string searchString,
            string currentFilter,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder == null ? "" : sortOrder;
            ViewData["TitleParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["InStockParm"] = sortOrder == "inStock_asc" ? "inStock_desc" : "inStock_asc";
            ViewData["ReceivedNumber"] = sortOrder == "receivedNumber_asc" ? "receivedNumber_desc" : "receivedNumber_asc";
            ViewData["ReceivedDate"] = sortOrder == "receivedDate_asc" ? "receivedDate_desc" : "receivedDate_asc";
            ViewData["OrderNumber"] = sortOrder == "orderNumber_asc" ? "orderNumber_desc" : "orderNumber_asc";
            ViewData["OrderedDate"] = sortOrder == "orderedDate_asc" ? "orderedDate_desc" : "orderedDate_asc";
            ViewData["ShippingNumber"] = sortOrder == "shippingNumber_asc" ? "shippingNumber_desc" : "shippingNumber_asc";
            ViewData["ShippedDate"] = sortOrder == "shippedDate_asc" ? "shippedDate_desc" : "shippedDate_asc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            IQueryable<InventoryItem> inventoryItems = _db.InventoryItems;

            if (!String.IsNullOrEmpty(searchString))
            {
                inventoryItems = inventoryItems.Where(s => s.BoardGame.Title.Contains(searchString));
            } 
            

            switch (sortOrder)
            {
                case "title_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.BoardGame.Title);
                    break;
                case "inStock_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.InStock);
                    break;
                case "inStock_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.InStock);
                    break;
                case "receivedNumber_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.ReceivedNumber);
                    break;
                case "receivedNumber_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.ReceivedNumber);
                    break;
                case "receivedDate_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.ReceivedDate);
                    break;
                case "receivedDate_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.ReceivedDate);
                    break;
                case "orderNumber_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.OrderNumber);
                    break;
                case "orderNumber_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.OrderNumber);
                    break;
                case "orderedDate_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.OrderedDate);
                    break;
                case "orderedDate_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.OrderedDate);
                    break;
                case "shippingNumber_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.ShippingNumber);
                    break;
                case "shippingNumber_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.ShippingNumber);
                    break;
                case "shippedDate_asc":
                    inventoryItems = inventoryItems.OrderBy(s => s.ShippedDate);
                    break;
                case "shippedDate_desc":
                    inventoryItems = inventoryItems.OrderByDescending(s => s.ShippedDate);
                    break;
                default:
                    inventoryItems = inventoryItems.OrderBy(s => s.BoardGame.Title);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<InventoryItem>.CreateAsync(inventoryItems.Include(i => i.BoardGame).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("InStock","ReceivedDate","OrderedDate",
            "ShippedDate","BoardGameID")] InventoryItem inventoryItem)
        {
            if (ModelState.IsValid)
            {
                var rand = new Random();
                var uid = rand.Next(100000, 1000000);

                var emptyInventoryItem = new InventoryItem();

                if (await TryUpdateModelAsync<InventoryItem>(
                    emptyInventoryItem,
                    "inventoryitem",
                    i => i.InStock,
                    i => i.ReceivedNumber,
                    i => i.ReceivedDate,
                    i => i.OrderedDate,
                    i => i.ShippedDate,
                    i => i.BoardGameID))
                {
                    emptyInventoryItem.ReceivedNumber = uid;
                    _db.InventoryItems.Add(emptyInventoryItem);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(inventoryItem);
        }

        public async Task<IActionResult> Create()
        {
            InventoryItem inventoryItem = new InventoryItem()
            {
                ReceivedDate = DateTime.UtcNow.Date
            };

            IEnumerable<BoardGame> boardGames = await _db.BoardGames
                .OrderBy(b => b.Title)
                .ToListAsync();

            InventoryItemDetailsViewModel inventoryItemDetailsViewModel = new InventoryItemDetailsViewModel()
            {
                InventoryItem = inventoryItem,
                BoardGame = boardGames
            };

            return View(inventoryItemDetailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID","InStock","ReceivedDate","BoardGameID")] InventoryItem inventoryItem)
        {
            var inventoryItemToUpdate = await _db.InventoryItems.FindAsync(id);

            if(inventoryItemToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(await TryUpdateModelAsync<InventoryItem>(
                    inventoryItemToUpdate,
                    "inventoryitem",
                    i => i.InStock,
                    i => i.ReceivedDate,
                    i => i.BoardGameID))
                {
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            IEnumerable<BoardGame> boardGames = await _db.BoardGames
                .OrderBy(b => b.Title)
                .ToListAsync();

            InventoryItemDetailsViewModel inventoryItemDetailsViewModel = new InventoryItemDetailsViewModel()
            {
                InventoryItem = inventoryItem,
                BoardGame = boardGames
            };

            return View(inventoryItemDetailsViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            InventoryItem inventoryItem = await _db.InventoryItems.FirstOrDefaultAsync(i => i.ID == id);

            if(inventoryItem == null)
            {
                return NotFound();
            }

            IEnumerable<BoardGame> boardGames = await _db.BoardGames
                .OrderBy(b => b.Title)
                .ToListAsync();

            InventoryItemDetailsViewModel inventoryItemDetailsViewModel = new InventoryItemDetailsViewModel()
            {
                InventoryItem = inventoryItem,
                BoardGame = boardGames
            };

            return View(inventoryItemDetailsViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _db.InventoryItems.FindAsync(id);

            if(inventoryItem == null)
            {
                return NotFound();
            }

            _db.InventoryItems.Remove(inventoryItem);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _db.InventoryItems
                .Include(i => i.BoardGame)
                    .ThenInclude(b => b.Brand)
                .FirstOrDefaultAsync(i => i.ID == id);
                
            if(inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _db.InventoryItems
                .Include(i => i.BoardGame)
                    .ThenInclude(b => b.Brand)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }
    }
}
