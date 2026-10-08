using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
             var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status)
                .ToListAsync();

            return View("~/Views/ProductClient/Index.cshtml", products);
        }
    }
}