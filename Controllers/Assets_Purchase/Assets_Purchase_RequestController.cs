using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nets_System.Data;
using Nets_System.Models;

namespace Nets_System.Controllers.Assets_Purchase
{
    public class Assets_Purchase_RequestController : Controller
    {
        private readonly Nets_SystemContext _context;

        public Assets_Purchase_RequestController(Nets_SystemContext context)
        {
            _context = context;
        }

        // GET: Assets_Purchase_Request?branchId=1  (gọi từ Branches: chọn branch → xem yêu cầu mua sắm của branch đó)
        public async Task<IActionResult> Index(int? branchId)
        {
            if (!branchId.HasValue)
            {
                return RedirectToAction(nameof(BranchesController.Index), "Branches");
            }

            var branch = await _context.Branches.FindAsync(branchId.Value);
            if (branch == null)
            {
                return NotFound();
            }

            var requests = await _context.AssetPurchaseRequests
                .Include(a => a.BranchManager)
                .ThenInclude(bm => bm!.Branch)
                .Where(a => a.BranchManager != null && a.BranchManager.Branch_Id == branchId.Value)
                .OrderByDescending(a => a.Created_Time)
                .ToListAsync();

            ViewBag.BranchId = branch.Id;
            ViewBag.BranchName = branch.Name;
            return View(requests);
        }

        // GET: Assets_Purchase_Request/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.AssetPurchaseRequests
                .Include(a => a.BranchManager)
                .Include(a => a.Items!)
                .ThenInclude(i => i.Asset_Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.BranchManager != null)
            {
                ViewBag.BranchId = request.BranchManager.Branch_Id;
            }

            ViewBag.ShowApproveButtons = false;
            return View(request);
        }

        // GET: Assets_Purchase_Request/Approve/5 - Phê duyệt (hiển thị chi tiết + nút Chấp nhận / Từ chối khi Status = Pending)
        public async Task<IActionResult> Approve(int? id, int? branchId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.AssetPurchaseRequests
                .Include(a => a.BranchManager)
                .Include(a => a.Items!)
                .ThenInclude(i => i.Asset_Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            ViewBag.BranchId = request.BranchManager?.Branch_Id ?? branchId;
            ViewBag.ShowApproveButtons = string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase);
            return View("Details", request);
        }

        // POST: Assets_Purchase_Request/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id, int? branchId)
        {
            var request = await _context.AssetPurchaseRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Approved";
            request.Last_Modified_Time = DateTime.Now;
            await _context.SaveChangesAsync();

            if (branchId.HasValue)
            {
                return RedirectToAction(nameof(Index), new { branchId = branchId.Value });
            }
            return RedirectToAction(nameof(BranchesController.Index), "Branches");
        }

        // POST: Assets_Purchase_Request/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, int? branchId)
        {
            var request = await _context.AssetPurchaseRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Rejected";
            request.Last_Modified_Time = DateTime.Now;
            await _context.SaveChangesAsync();

            if (branchId.HasValue)
            {
                return RedirectToAction(nameof(Index), new { branchId = branchId.Value });
            }
            return RedirectToAction(nameof(BranchesController.Index), "Branches");
        }
    }
}
