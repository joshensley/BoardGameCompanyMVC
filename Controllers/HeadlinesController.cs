using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    public class HeadlinesController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly IWebHostEnvironment _environment;

        public HeadlinesController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {

            IList<Headlines> Headlines = await _db.Headlines
                .Include(i => i.BoardGame)
                .ToListAsync();

            return View(Headlines);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ID","BoardGameID","IsActive", 
            "ImageFilePath", "FormFileUpload")] Headlines headlines)
        {
            var headlineToUpdate = await _db.Headlines.FindAsync(id);

            if (headlineToUpdate == null)
            {
                return NotFound();
            }

            var filepath = headlines.ImageFilePath;
            var oldFilePath = headlines.ImageFilePath;
            if (ModelState.IsValid)
            {
                if (headlines.FormFileUpload != null) {
                    var rand = new Random();
                    var randNum = rand.Next(100000, 1000000);
                    var fileName = randNum.ToString() + "_" + headlines.FormFileUpload.FileName;
                    filepath = $"/images/headline/{fileName}";
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/headline/", fileName);

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await headlines.FormFileUpload.CopyToAsync(fileStream);
                    }

                    var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{headlineToUpdate.ImageFilePath}");
                    if (System.IO.File.Exists(deleteFile) && (oldFilePath != "/images/headline/defaultHeadline/gray.jpg"))
                    {
                        System.IO.File.Delete(deleteFile);
                    }
                }

                if (await TryUpdateModelAsync<Headlines>(
                    headlineToUpdate,
                    "",
                    i => i.IsActive,
                    i => i.ImageFilePath))
                {
                    if(headlines.FormFileUpload != null)
                    {
                        headlineToUpdate.ImageFilePath = filepath;
                    }

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(headlines);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Headlines Headlines = await _db.Headlines
                .Include(i => i.BoardGame)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (Headlines == null)
            {
                return NotFound();
            }

            return View(Headlines);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID","BoardGameID","IsActive",
            "ImageFilePath","FormFileUpload")] Headlines headline)
        {

            var errors = ModelState
               .Where(x => x.Value.Errors.Count > 0)
               .Select(x => new { x.Key, x.Value.Errors })
               .ToArray();

            if (ModelState.IsValid)
            {
                var filepath = "/images/headline/defaultHeadline/gray.jpg";
                if (headline.FormFileUpload != null)
                {
                    var rand = new Random();
                    var randNum = rand.Next(100000, 1000000);
                    var fileName = randNum.ToString() + "_" + headline.FormFileUpload.FileName;
                    filepath = $"/images/headline/{fileName}";
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/headline/", fileName);

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await headline.FormFileUpload.CopyToAsync(fileStream);
                    }
                }

                var emptyHeadlines = new Headlines();

                if (await TryUpdateModelAsync<Headlines>(
                    emptyHeadlines,
                    "headline",
                    i => i.BoardGameID,
                    i => i.IsActive,
                    i => i.ImageFilePath))
                {
                    emptyHeadlines.ImageFilePath = filepath;
                    _db.Add(emptyHeadlines);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                
            }
            

            IEnumerable<BoardGame> boardGames = await _db.BoardGames
               .OrderBy(i => i.Title)
               .ToListAsync();

            HeadlinesViewModel headlinesViewModel = new HeadlinesViewModel()
            {
                Headline = headline,
                BoardGames = boardGames
            };

            return View(headlinesViewModel);
        }


        public async Task<IActionResult> Create()
        {

            Headlines headline = new Headlines();

            IEnumerable<BoardGame> boardGames = await _db.BoardGames
                .OrderBy(i => i.Title)
                .ToListAsync();

            HeadlinesViewModel headlinesViewModel = new HeadlinesViewModel()
            {
                Headline = headline,
                BoardGames = boardGames
            };

            return View(headlinesViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var headline = await _db.Headlines.FindAsync(id);

            if (headline == null)
            {
                return NotFound();
            }

            _db.Headlines.Remove(headline);

            var deleteFile = Path.Combine(_environment.ContentRootPath, $"wwwroot{headline.ImageFilePath}");

            if (System.IO.File.Exists(deleteFile) && (headline.ImageFilePath != "/images/headline/defaultHeadline/gray.jpg"))
            {
                System.IO.File.Delete(deleteFile);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Delete(int? id)
        {
            Headlines headlines = await _db.Headlines
                .Include(i => i.BoardGame)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (headlines == null)
            {
                return NotFound();
            }

            return View(headlines);
        }
    }
}
