using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public IActionResult Flot()
        {
            return View();
        }

        [Authorize]
        public IActionResult Morris()
        {
            return View();
        }

        [Authorize]
        public IActionResult Tables()
        {
            return View();
        }

        [Authorize]
        public IActionResult Forms()
        {
            return View();
        }

        [Authorize]
        public IActionResult Panels_wells()
        {
            return View();
        }

        [Authorize]
        public IActionResult Buttons()
        {
            return View();
        }

        [Authorize]
        public IActionResult Notifications()
        {
            return View();
        }

        [Authorize]
        public IActionResult Typography()
        {
            return View();
        }

        [Authorize]
        public IActionResult Icons()
        {
            return View();
        }

        [Authorize]
        public IActionResult Grid()
        {
            return View();
        }

        [Authorize]
        public IActionResult blank()
        {
            return View();
        }


        // =========================
        // ORDERS
        // =========================

        [Authorize]
        public async Task<IActionResult> Order()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(
                "~/Views/Admin/Order/Index.cshtml",
                orders);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(
                "~/Views/Admin/Order/Details.cshtml",
                order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(OrderDetails),
                new { id = id });
        }


        // =========================
        // LOGIN
        // =========================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password)
        {
            if (email == "admin@gmail.com" &&
                password == "123456")
            {
                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        email)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults
                        .AuthenticationScheme);

                var principal =
                    new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults
                        .AuthenticationScheme,
                    principal);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error =
                "Email hoặc password không đúng";

            return View();
        }


        // =========================
        // LOGOUT
        // =========================

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}