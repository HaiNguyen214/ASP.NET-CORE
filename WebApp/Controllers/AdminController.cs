using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Flot() {
            return View();
        }
        public IActionResult Morris()
        {
            return View();
        }
        public IActionResult Tables()
        {
            return View();
        }
        public IActionResult Forms()
        {
            return View();
        }
        public IActionResult Panels_wells()
        {
            return View();
        }
        public IActionResult Buttons()
        {
            return View();
        }
        public IActionResult Notifications()
        {
            return View();
        }
        public IActionResult Typography()
        {
            return View();
        }
        public IActionResult Icons()
        {
            return View();
        }
        public IActionResult Grid()
        {
            return View();
        }
        public IActionResult blank()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "admin@gmail.com" && password == "123456")
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Email hoặc password không đúng";

            return View();
        }
    }
}