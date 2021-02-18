using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    public class UserShippingController : Controller
    {
        public readonly ApplicationDbContext _db;

        public UserShippingController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            IQueryable<UserOrderGroup> data =
                from inventoryItems in _db.InventoryItems
                where inventoryItems.OrderedDate != null
                    && inventoryItems.ShippingNumber == null
                    && inventoryItems.InStock == false
                join boardGame in _db.BoardGames
                    on inventoryItems.BoardGameID equals boardGame.ID
                group inventoryItems by new
                {
                    inventoryItems.OrderNumber,
                    inventoryItems.OrderedDate,

                } into userOrderGroupIQ
                orderby userOrderGroupIQ.Key.OrderedDate ascending
                select new UserOrderGroup()

                {
                    OrderNumber = (int)userOrderGroupIQ.Key.OrderNumber,
                    OrderDate = userOrderGroupIQ.Key.OrderedDate,
                    OrderCount = userOrderGroupIQ.Count()
                };
            
            int pageSize = 5;
            return View(await PaginatedList<UserOrderGroup>.CreateAsync(data.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                var rand = new Random();
                var uid = rand.Next(100000, 1000000);

                var inventoryItemToUpdate = await _db.InventoryItems
                    .Where(i => i.OrderNumber == id)
                    .ToListAsync();

                foreach (var item in inventoryItemToUpdate)
                {
                    await TryUpdateModelAsync<InventoryItem>(
                        item,
                        "",
                        i => i.ShippingNumber,
                        i => i.ShippedDate);
                    {
                        item.ShippingNumber = uid;
                        item.ShippedDate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                        await _db.SaveChangesAsync();
                        
                    }
                }
                return RedirectToAction("Confirmation", new { id = id });
            }

            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            };

            IEnumerable<InventoryItem> inventoryItem = await _db.InventoryItems
                .Where(i => i.OrderNumber == id)
                .Include(i => i.BoardGame)
                .ThenInclude(i => i.Brand)
                .ToListAsync();

            if (inventoryItem == null)
            {
                return NotFound();
            };

            return View(inventoryItem);
        }

        public IActionResult Confirmation(int id)
        {
            return View();
        }
    }
}
