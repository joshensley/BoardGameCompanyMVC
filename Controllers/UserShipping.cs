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
    public class UserShipping : Controller
    {
        public readonly ApplicationDbContext _db;

        public UserShipping(ApplicationDbContext db)
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

        public IActionResult Edit(int id)
        {
            return View();
        }
    }
}
