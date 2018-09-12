using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirmManager.Data;
using FirmManager.Models;
using Microsoft.AspNetCore.Authorization;
using System;

namespace FirmManager.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ProductsController : Controller
    {
        private readonly FirmContext context;

        public ProductsController(FirmContext context)
        {
            this.context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["CategorySortParm"] = sortOrder == "category_asc" ? "category_desc" : "category_asc";
            ViewData["SizeSortParm"] = sortOrder == "size_asc" ? "size_desc" : "size_asc";
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["UnitsInStockSortParm"] = sortOrder == "units_asc" ? "units_desc" : "units_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (page < 1)
            {
                page = 1;
            }

            ViewData["CurrentFilter"] = searchString;

            var products = from p in context.Products
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Title.ToLower().Contains(searchString.ToLower())
                                       || p.Category.ToLower().Contains(searchString.ToLower())
                                       || p.Size.ToLower().Contains(searchString.ToLower())
                                       || p.Price.ToString().ToLower().Contains(searchString.ToLower())
                                       || p.UnitsInStock.ToString().ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    products = products.OrderByDescending(p => p.Title);
                    break;
                case "category_asc":
                    products = products.OrderBy(p => p.Category);
                    break;
                case "category_desc":
                    products = products.OrderByDescending(p => p.Category);
                    break;
                case "size_asc":
                    products = products.OrderBy(p => p.Size);
                    break;
                case "size_desc":
                    products = products.OrderByDescending(p => p.Size);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "units_asc":
                    products = products.OrderBy(p => p.UnitsInStock);
                    break;
                case "units_desc":
                    products = products.OrderByDescending(p => p.UnitsInStock);
                    break;
                default:
                    products = products.OrderBy(p => p.Title);
                    break;
            }

            int numberOfLines = 12;

            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), page ?? 1, numberOfLines));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await context.Products
                .SingleOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Title,Category,Size,Price,UnitsInStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                context.Add(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await context.Products.SingleOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Title,Category,Size,Price,UnitsInStock")] Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(product);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await context.Products
                .SingleOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await context.Products.SingleOrDefaultAsync(m => m.ID == id);
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return context.Products.Any(e => e.ID == id);
        }
    }
}