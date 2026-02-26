using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Assets_Purchase
{
    public class Assets_Purchase_RequestController : Controller
    {
        private readonly WebApplication1Context _context;

        public Assets_Purchase_RequestController(WebApplication1Context context)
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

        // GET: Assets_Purchase_Request/Create?branchId=1
        public async Task<IActionResult> Create(int? branchId)
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

            var branchManager = await _context.BranchManagers
                .FirstOrDefaultAsync(bm => bm.Branch_Id == branchId.Value);
            if (branchManager == null)
            {
                return BadRequest("Chi nhánh này chưa có Branch Manager, không thể tạo yêu cầu mua sắm.");
            }

            var model = new AssetPurchaseRequestCreateViewModel
            {
                BranchId = branch.Id
            };

            // Khởi tạo 1 dòng trống ban đầu để nhập loại tài sản
            model.Items.Add(new AssetPurchaseRequestItemInput());

            ViewBag.BranchId = branch.Id;
            ViewBag.BranchName = branch.Name;
            ViewBag.Categories = new SelectList(_context.AssetCategories, "Id", "Name");
            return View(model);
        }

        // POST: Assets_Purchase_Request/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetPurchaseRequestCreateViewModel model)
        {
            var branch = await _context.Branches.FindAsync(model.BranchId);
            if (branch == null)
            {
                return NotFound();
            }

            var branchManager = await _context.BranchManagers
                .FirstOrDefaultAsync(bm => bm.Branch_Id == model.BranchId);
            if (branchManager == null)
            {
                ModelState.AddModelError(string.Empty, "Chi nhánh này chưa có Branch Manager, không thể tạo yêu cầu.");
            }

            // Lọc các dòng item hợp lệ (có chọn loại và số lượng > 0)
            var validItems = model.Items?
                .Where(i => i.Asset_Category_Id > 0 && i.Quantity > 0)
                .ToList() ?? new List<AssetPurchaseRequestItemInput>();

            if (!validItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Cần nhập ít nhất một loại tài sản với số lượng > 0.");
            }

            if (!ModelState.IsValid || branchManager == null)
            {
                ViewBag.BranchId = model.BranchId;
                ViewBag.BranchName = branch.Name;
                ViewBag.Categories = new SelectList(_context.AssetCategories, "Id", "Name");
                return View(model);
            }

            var request = new Assets_Purchase_Request
            {
                Branch_Manager_Id = branchManager.Id,
                Reason = model.Reason,
                Note = model.Note,
                Status = "Chờ duyệt",
                Created_Time = DateTime.Now,
                Last_Modified_Time = DateTime.Now
            };

            _context.AssetPurchaseRequests.Add(request);
            await _context.SaveChangesAsync();

            // Thêm các item đi kèm yêu cầu
            foreach (var item in validItems)
            {
                var entityItem = new Asset_Purchase_Request_Item
                {
                    Asset_Purchase_Request_Id = request.Id,
                    Asset_Category_Id = item.Asset_Category_Id,
                    Quantity = item.Quantity,
                    Created_Time = DateTime.Now,
                    Last_Modified_Time = DateTime.Now
                };
                _context.AssetPurchaseRequestItems.Add(entityItem);
            }

            if (validItems.Any())
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { branchId = model.BranchId });
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
            ViewBag.ShowApproveButtons = string.Equals(request.Status, "Chờ duyệt", StringComparison.OrdinalIgnoreCase);
            return View("Details", request);
        }

        // GET: Assets_Purchase_Request/Quote/5 - Báo giá cho yêu cầu đã Approved
        public async Task<IActionResult> Quote(int id, int? branchId)
        {
            var request = await _context.AssetPurchaseRequests
                .Include(r => r.BranchManager)
                    .ThenInclude(bm => bm!.Branch)
                .Include(r => r.Items!)
                    .ThenInclude(i => i.Asset_Category)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            if (!string.Equals(request.Status, "Đã duyệt", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Chỉ có thể báo giá cho các yêu cầu đã ở trạng thái Approved.");
            }

            ViewBag.BranchId = branchId ?? request.BranchManager?.Branch_Id;
            ViewBag.BranchName = request.BranchManager?.Branch?.Name ?? "Chi nhánh";
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");

            return View(request);
        }

        // POST: Assets_Purchase_Request/Quote/5 - Lưu báo giá vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Quote(AssetQuotationSaveViewModel model)
        {
            var request = await _context.AssetPurchaseRequests
                .Include(r => r.Items!)
                .FirstOrDefaultAsync(r => r.Id == model.RequestId);

            if (request == null)
            {
                return NotFound();
            }

            // Chuẩn hóa danh sách dòng báo giá hợp lệ
            var validLines = model.QuotationItems?
                .Where(l => l.UnitPrice > 0)
                .ToList() ?? new List<AssetQuotationItemInput>();

            if (!validLines.Any())
            {
                ModelState.AddModelError(string.Empty, "Cần nhập ít nhất một đơn giá hợp lệ.");
            }

            // Xác định / tạo nhà cung cấp
            Supplier? supplier = null;
            if (model.SupplierId.HasValue)
            {
                supplier = await _context.Suppliers.FindAsync(model.SupplierId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(model.SupplierName))
            {
                var name = model.SupplierName.Trim();
                supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.Name == name);

                if (supplier == null)
                {
                    supplier = new Supplier
                    {
                        Name = name,
                        Email = string.Empty,
                        Phone = string.Empty,
                        Address = string.Empty,
                        Created_Time = DateTime.Now,
                        Last_Modified_Time = DateTime.Now
                    };
                    _context.Suppliers.Add(supplier);
                    await _context.SaveChangesAsync();
                }

                model.SupplierId = supplier.Id;
            }

            if (supplier == null)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn hoặc nhập nhà cung cấp.");
            }

            if (!ModelState.IsValid)
            {
                // Trả lại view với dữ liệu hiện tại
                var branch = await _context.Branches.FindAsync(model.BranchId);
                ViewBag.BranchId = model.BranchId;
                ViewBag.BranchName = branch?.Name ?? "Chi nhánh";
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");

                // Tải lại request để hiển thị
                var reloadRequest = await _context.AssetPurchaseRequests
                    .Include(r => r.Items!)
                        .ThenInclude(i => i.Asset_Category)
                    .FirstOrDefaultAsync(r => r.Id == model.RequestId);

                return View(reloadRequest!);
            }

            // Tạm thời dùng User_Id = 1 (cần tích hợp với hệ thống user thực tế)
            var userId = 1;

            var quotation = new Asset_Quotation
            {
                Purchase_Id = request.Id,
                User_Id = userId,
                Supplier_Id = supplier.Id,
                Description = $"Báo giá cho yêu cầu #{request.Id}",
                Status = "New",
                Created_Time = DateTime.Now,
                Last_Modified_Time = DateTime.Now
            };

            _context.Set<Asset_Quotation>().Add(quotation);
            await _context.SaveChangesAsync();

            foreach (var line in validLines)
            {
                var item = new Asset_Quotation_Item
                {
                    Request_Quotation_Id = quotation.Id,
                    Unit_Price = line.UnitPrice,
                    DiscountPercent = line.DiscountPercent,
                    Created_Time = DateTime.Now,
                    Last_Modified_Time = DateTime.Now
                };
                _context.Set<Asset_Quotation_Item>().Add(item);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { branchId = model.BranchId });
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

            request.Status = "Đã duyệt";
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

            request.Status = "Đã từ chối";
            request.Last_Modified_Time = DateTime.Now;
            await _context.SaveChangesAsync();

            if (branchId.HasValue)
            {
                return RedirectToAction(nameof(Index), new { branchId = branchId.Value });
            }
            return RedirectToAction(nameof(BranchesController.Index), "Branches");
        }

        // GET: Assets_Purchase_Request/Edit/5
        public async Task<IActionResult> Edit(int? id, int? branchId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.AssetPurchaseRequests
                .Include(r => r.BranchManager)
                .Include(r => r.Items!)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            var effectiveBranchId = branchId ?? request.BranchManager?.Branch_Id ?? 0;

            var model = new AssetPurchaseRequestEditViewModel
            {
                Id = request.Id,
                BranchId = effectiveBranchId,
                Reason = request.Reason,
                Status = request.Status,
                Note = request.Note,
                Items = request.Items?
                    .Select(i => new AssetPurchaseRequestItemInput
                    {
                        Asset_Category_Id = i.Asset_Category_Id,
                        Quantity = i.Quantity
                    })
                    .ToList() ?? new List<AssetPurchaseRequestItemInput>()
            };

            if (!model.Items.Any())
            {
                model.Items.Add(new AssetPurchaseRequestItemInput());
            }

            var branch = await _context.Branches.FindAsync(effectiveBranchId);
            ViewBag.BranchId = effectiveBranchId;
            ViewBag.BranchName = branch?.Name ?? "Chi nhánh";
            ViewBag.Categories = new SelectList(_context.AssetCategories, "Id", "Name");

            return View(model);
        }

        // POST: Assets_Purchase_Request/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AssetPurchaseRequestEditViewModel model)
        {
            var request = await _context.AssetPurchaseRequests
                .Include(r => r.Items!)
                .FirstOrDefaultAsync(r => r.Id == model.Id);
            if (request == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(model.BranchId);
            if (branch == null)
            {
                return NotFound();
            }

            var validItems = model.Items?
                .Where(i => i.Asset_Category_Id > 0 && i.Quantity > 0)
                .ToList() ?? new List<AssetPurchaseRequestItemInput>();

            if (!validItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Cần nhập ít nhất một loại tài sản với số lượng > 0.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.BranchId = model.BranchId;
                ViewBag.BranchName = branch.Name;
                ViewBag.Categories = new SelectList(_context.AssetCategories, "Id", "Name");
                return View(model);
            }

            request.Reason = model.Reason;
            request.Status = model.Status;
            request.Note = model.Note;
            request.Last_Modified_Time = DateTime.Now;

            // Xóa các item cũ và thêm lại từ danh sách mới
            if (request.Items != null && request.Items.Any())
            {
                _context.AssetPurchaseRequestItems.RemoveRange(request.Items);
            }

            foreach (var item in validItems)
            {
                var entityItem = new Asset_Purchase_Request_Item
                {
                    Asset_Purchase_Request_Id = request.Id,
                    Asset_Category_Id = item.Asset_Category_Id,
                    Quantity = item.Quantity,
                    Created_Time = DateTime.Now,
                    Last_Modified_Time = DateTime.Now
                };
                _context.AssetPurchaseRequestItems.Add(entityItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { branchId = model.BranchId });
        }
    }
}
