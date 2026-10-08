using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Controllers.client
{
    public class ProductClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Shop(int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(
                    p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();

            ViewBag.Categories =
                await _context.Categories.ToListAsync();

            return View(
                "~/Views/ProductClient/Shop.cshtml",
                products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(
                    p => p.Id == id && p.Status);

            if (product == null)
            {
                return NotFound();
            }

            return View(
                "~/Views/ProductClient/Details.cshtml",
                product);
        }

        public async Task<IActionResult> AddToCart(
            int id,
            int quantity = 1)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    p => p.Id == id && p.Status);

            if (product == null)
            {
                return NotFound();
            }

            if (product.Quantity <= 0)
            {
                return RedirectToAction(
                    nameof(Details),
                    new { id = id });
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            if (quantity > product.Quantity)
            {
                quantity = product.Quantity;
            }

            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            var item = cart.FirstOrDefault(
                x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity += quantity;

                if (item.Quantity > product.Quantity)
                {
                    item.Quantity = product.Quantity;
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Image = product.ImgURL,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            HttpContext.Session.SetObject(
                "Cart",
                cart);

            return RedirectToAction(
                nameof(Cart));
        }

        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            var item = cart.FirstOrDefault(
                x => x.ProductId == id);

            if (item != null)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(
                        p => p.Id == id);

                if (product != null)
                {
                    if (product.Quantity <= 0)
                    {
                        cart.Remove(item);
                    }
                    else if (item.Quantity < product.Quantity)
                    {
                        item.Quantity++;
                    }
                }
            }

            HttpContext.Session.SetObject(
                "Cart",
                cart);

            return RedirectToAction(
                nameof(Cart));
        }

        public IActionResult DecreaseQuantity(int id)
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            var item = cart.FirstOrDefault(
                x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            HttpContext.Session.SetObject(
                "Cart",
                cart);

            return RedirectToAction(
                nameof(Cart));
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            var item = cart.FirstOrDefault(
                x => x.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.SetObject(
                "Cart",
                cart);

            return RedirectToAction(
                nameof(Cart));
        }

        public IActionResult ProductDetail()
        {
            return RedirectToAction(
                nameof(Shop));
        }

        public IActionResult Cart()
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            return View(
                "~/Views/ProductClient/Cart.cshtml",
                cart);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            if (!cart.Any())
            {
                return RedirectToAction(
                    nameof(Cart));
            }

            return View(
                "~/Views/ProductClient/Checkout.cshtml",
                new Order());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            Order order)
        {
            var cart = HttpContext.Session
                .GetObject<List<CartItem>>("Cart")
                ?? new List<CartItem>();

            if (!cart.Any())
            {
                return RedirectToAction(
                    nameof(Cart));
            }

            ModelState.Remove("OrderDetails");

            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/ProductClient/Checkout.cshtml",
                    order);
            }

            order.OrderDate = DateTime.Now;
            order.Status = "Pending";
            order.TotalAmount = 0;

            foreach (var item in cart)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(
                        p => p.Id == item.ProductId);

                if (product == null)
                {
                    continue;
                }

                if (product.Quantity <= 0)
                {
                    ModelState.AddModelError(
                        "",
                        $"Sản phẩm {product.Name} đã hết hàng.");

                    return View(
                        "~/Views/ProductClient/Checkout.cshtml",
                        order);
                }

                if (product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError(
                        "",
                        $"Sản phẩm {product.Name} chỉ còn {product.Quantity} sản phẩm.");

                    return View(
                        "~/Views/ProductClient/Checkout.cshtml",
                        order);
                }

                var orderDetail = new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                order.OrderDetails.Add(
                    orderDetail);

                order.TotalAmount +=
                    product.Price * item.Quantity;

                product.Quantity -= item.Quantity;
            }

            if (!order.OrderDetails.Any())
            {
                return RedirectToAction(
                    nameof(Cart));
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction(
                nameof(OrderSuccess),
                new { id = order.Id });
        }

        public async Task<IActionResult> OrderSuccess(
            int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(
                    o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(
                "~/Views/ProductClient/OrderSuccess.cshtml",
                order);
        }
    }
}