using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nets_System.Data;

namespace Nets_System.Controllers
{
    public class BranchesController : Controller
    {
        private readonly Nets_SystemContext _context;

        public BranchesController(Nets_SystemContext context)
        {
            _context = context;
        }

        // GET: Branches - Danh sách chi nhánh (admin vào đây trước, sau đó chọn branch để xem yêu cầu mua sắm)
        public async Task<IActionResult> Index()
        {
            var branches = await _context.Branches
                .OrderBy(b => b.Name)
                .ToListAsync();
            return View(branches);
        }
    }
}
