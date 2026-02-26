using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Assets
{
    public class AssetsController : Controller
    {
        private readonly WebApplication1Context _context;

        public AssetsController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets
                .Include(a => a.Category)
                .ToListAsync();
            return View(assets);
        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // GET: Assets/Create
        public IActionResult Create()
        {
            ViewBag.StatusList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Đang sử dụng", Text = "Đang sử dụng" },
                new SelectListItem { Value = "Có sẵn", Text = "Có sẵn" },
                new SelectListItem { Value = "Bảo trì", Text = "Bảo trì" }
            }, "Value", "Text");
            ViewData["Category_Id"] = new SelectList(_context.AssetCategories, "Id", "Name");
            return View();
        }

        // POST: Assets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Branch_Id,Category_Id,Name,Barcode,Status,Created_Time,Last_Modified_Time")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                asset.Created_Time = DateTime.Now;
                asset.Last_Modified_Time = DateTime.Now;
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Category_Id"] = new SelectList(_context.AssetCategories, "Id", "Name", asset.Category_Id);
            return View(asset);
        }

        // GET: Assets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets.FindAsync(id);
            ViewBag.StatusList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Đang sử dụng", Text = "Đang sử dụng" },
                new SelectListItem { Value = "Có sẵn", Text = "Có sẵn" },
                new SelectListItem { Value = "Bảo trì", Text = "Bảo trì" }
            }, "Value", "Text", asset.Status);
            if (asset == null)
            {
                return NotFound();
            }
            ViewData["Category_Id"] = new SelectList(_context.AssetCategories, "Id", "Name", asset.Category_Id);
            return View(asset);
        }

        // POST: Assets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Branch_Id,Category_Id,Name,Barcode,Status,Created_Time,Last_Modified_Time")] Asset asset)
        {
            if (id != asset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    asset.Last_Modified_Time = DateTime.Now;
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetExists(asset.Id))
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
            ViewData["Category_Id"] = new SelectList(_context.AssetCategories, "Id", "Name", asset.Category_Id);
            return View(asset);
        }

        // GET: Assets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.Id == id);
        }
    }
}
