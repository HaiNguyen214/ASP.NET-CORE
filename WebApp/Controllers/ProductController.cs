using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        // GET: /Product/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(
                _context.Categories,
                "Id",
                "Name");

            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Product product,
            IFormFile? image)
        {
            // Category được lấy thông qua CategoryId
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(image.FileName);

                    string folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/products");

                    Directory.CreateDirectory(folder);

                    string filePath =
                        Path.Combine(folder, fileName);

                    using (var stream =
                        new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    product.ImgURL =
                        "/images/products/" + fileName;
                }

                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        // GET: /Product/Edit/1
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product =
                await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            ViewBag.CategoryId = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        // POST: /Product/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product product,
            IFormFile? image)
        {
            if (id != product.Id)
                return NotFound();

            // Không validate navigation property Category
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                var oldProduct =
                    await _context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

                if (oldProduct == null)
                    return NotFound();

                if (image != null && image.Length > 0)
                {
                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(image.FileName);

                    string folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/products");

                    Directory.CreateDirectory(folder);

                    string filePath =
                        Path.Combine(folder, fileName);

                    using (var stream =
                        new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    product.ImgURL =
                        "/images/products/" + fileName;
                }
                else
                {
                    product.ImgURL =
                        oldProduct.ImgURL;
                }

                _context.Products.Update(product);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        // GET: /Product/Delete/1
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Product/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product =
                await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}