using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using Microsoft.AspNetCore.Authorization;
using BoardGameCompanyMVC.Utility;

namespace BoardGameCompanyMVC.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class UnitedStatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitedStatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UnitedStates
        public async Task<IActionResult> Index()
        {
            return View(await _context.UnitedStates.ToListAsync());
        }

        // GET: UnitedStates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitedStates = await _context.UnitedStates
                .FirstOrDefaultAsync(m => m.ID == id);
            if (unitedStates == null)
            {
                return NotFound();
            }

            return View(unitedStates);
        }

        // GET: UnitedStates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnitedStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,State")] UnitedStates unitedStates)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unitedStates);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unitedStates);
        }

        // GET: UnitedStates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitedStates = await _context.UnitedStates.FindAsync(id);
            if (unitedStates == null)
            {
                return NotFound();
            }
            return View(unitedStates);
        }

        // POST: UnitedStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,State")] UnitedStates unitedStates)
        {
            if (id != unitedStates.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unitedStates);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitedStatesExists(unitedStates.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unitedStates);
        }

        // GET: UnitedStates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitedStates = await _context.UnitedStates
                .FirstOrDefaultAsync(m => m.ID == id);
            if (unitedStates == null)
            {
                return NotFound();
            }

            return View(unitedStates);
        }

        // POST: UnitedStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unitedStates = await _context.UnitedStates.FindAsync(id);
            _context.UnitedStates.Remove(unitedStates);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitedStatesExists(int id)
        {
            return _context.UnitedStates.Any(e => e.ID == id);
        }
    }
}
