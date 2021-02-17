using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.Utility;
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
    public class BrandController : Controller
    {
        public readonly ApplicationDbContext _db;

        public BrandController(ApplicationDbContext db)
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
            ViewData["BrandNameParm"] = String.IsNullOrEmpty(sortOrder) ? "brandName_desc" : "";
            ViewData["CountParm"] = sortOrder == "count_desc" ? "count_asc" : "count_desc";

            if(searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            IQueryable<Brand> brands = _db.Brands.Include(b => b.BoardGames);

            if (!String.IsNullOrEmpty(searchString))
            {
                brands = brands.Where(b => b.BrandName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "brandName_desc":
                    brands = brands.OrderByDescending(s => s.BrandName);
                    break;
                case "count_asc":
                    brands = brands.OrderBy(s => s.BoardGames.Count());
                    break;
                case "count_desc":
                    brands = brands.OrderByDescending(s => s.BoardGames.Count());
                    break;
                default:
                    brands = brands.OrderBy(s => s.BrandName);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Brand>.CreateAsync(brands.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID","BrandName")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                var emptyBrand = new Brand();

                if (await TryUpdateModelAsync<Brand>(
                    emptyBrand,
                    "",
                    b => b.BrandName))
                {
                    _db.Brands.Add(emptyBrand);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
           
            return View(brand);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID", "BrandName")] Brand brand)
        {
            var brandToUpdate = await _db.Brands.FindAsync(id);

            if (brandToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(await TryUpdateModelAsync<Brand>(
                    brandToUpdate,
                    "",
                    b => b.BrandName))
                {
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(brandToUpdate);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _db.Brands.FirstOrDefaultAsync(b => b.ID == id);

            return View(brand);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var brand = await _db.Brands
                .Include(b => b.BoardGames)
                .FirstOrDefaultAsync(b => b.ID == id);

            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var brand = await _db.Brands.FindAsync(id);

            if(brand == null)
            {
                return NotFound();
            }

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _db.Brands.FirstOrDefaultAsync(b => b.ID == id);

            if(brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

    }
}
