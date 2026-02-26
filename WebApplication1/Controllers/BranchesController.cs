using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class BranchesController : Controller
    {
        private readonly WebApplication1Context _context;

        public BranchesController(WebApplication1Context context)
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
