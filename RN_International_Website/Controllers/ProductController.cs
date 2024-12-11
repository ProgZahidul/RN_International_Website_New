using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using RN_International_Website.Models;

namespace RN_International_Website.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            
            return View(products);
        }

        // GET: Product/Upsert/5 (5 is optional for creating or editing)
        public async Task<IActionResult> Upsert(int? id)
        {
            // Populate the category list
            var categoryList = await _context.Categories
                .Select(u => new SelectListItem
                {
                    Text = u.Name,  // Corrected property name
                    Value = u.Id.ToString()
                })
                .ToListAsync();

            // Pass the category list to the view using ViewBag
            //ViewBag.CategoryList = categoryList;
            ViewData["CategoryList"] = categoryList;
         

            // If `id` is null, we're creating a new product; otherwise, fetch the product for editing
            Product product = id == null ? new Product() : await _context.Products.FindAsync(id);

            // If the product doesn't exist, redirect to the list or return a 404 error
            if (id != null && product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Upsert(Product product, IFormFile? Image)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string wwwRootPath = _webHostEnvironment.WebRootPath;

        //        // Handle image upload
        //        if (Image != null)
        //        {
        //            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
        //            string uploadPath = Path.Combine(wwwRootPath, "images/productimages");

        //            // Ensure the directory exists
        //            Directory.CreateDirectory(uploadPath);

        //            // Delete old image if editing
        //            if (!string.IsNullOrEmpty(product.ImageUrl))
        //            {
        //                string oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('/'));
        //                if (System.IO.File.Exists(oldImagePath))
        //                {
        //                    try
        //                    {
        //                        System.IO.File.Delete(oldImagePath);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        // Log error for debugging
        //                        Console.WriteLine($"Error deleting old image: {ex.Message}");
        //                    }
        //                }
        //            }

        //            // Save the new image
        //            using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
        //            {
        //                await Image.CopyToAsync(fileStream);
        //            }
        //            product.ImageUrl = $"/images/productimages/{fileName}";
        //        }

        //        if (product.Id == 0)
        //        {
        //            // Create new product
        //            _context.Add(product);
        //        }
        //        else
        //        {
        //            // Update existing product
        //            _context.Update(product);
        //        }

        //        await _context.SaveChangesAsync();
        //        TempData["success"] = "Product saved successfully!";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        //    return View(product);
        //}
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category) // Include the Category if necessary
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Product product, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (Image != null)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Generate a unique file name for the new image
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    string uploadPath = Path.Combine(wwwRootPath, "images/productimages");

                    // Ensure the directory exists
                    Directory.CreateDirectory(uploadPath);

                    // Save the new image
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = $"/images/productimages/{fileName}";
                }

                // Add or update the product in the database
                if (product.Id == 0)
                {
                    _context.Add(product);
                }
                else
                {
                    _context.Update(product);
                }

                await _context.SaveChangesAsync();
                TempData["success"] = "Product saved successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }




        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Confirm delete view
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Delete associated image
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteImage(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string imagePath = Path.Combine(wwwRootPath, imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return Json(new { success = true, message = "Image deleted successfully" });
                }
            }
            return Json(new { success = false, message = "Image not found" });
        }

    }
}
