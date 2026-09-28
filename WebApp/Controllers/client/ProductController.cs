using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers.client
{
    public class ProductController : Controller
    {
        public IActionResult ProductDetail() { return View(); }
        public IActionResult Shop() { return View(); }
        public IActionResult Cart() { return View(); }
        public IActionResult Checkout() { return View(); }
    }
}
