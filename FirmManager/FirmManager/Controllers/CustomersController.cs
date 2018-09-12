using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirmManager.Data;
using FirmManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;

namespace FirmManager.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CustomersController : Controller
    {
        private readonly FirmContext context;
        private readonly IHostingEnvironment environment;

        public CustomersController(FirmContext context, IHostingEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        // GET: Customers
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["FirstNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewData["LastNameSortParm"] = sortOrder == "lastname_asc" ? "lastname_desc" : "lastname_asc";
            ViewData["AddressSortParm"] = sortOrder == "address_asc" ? "address_desc" : "address_asc";
            ViewData["CitySortParm"] = sortOrder == "city_asc" ? "city_desc" : "city_asc";
            ViewData["StateSortParm"] = sortOrder == "state_asc" ? "state_desc" : "state_asc";
            ViewData["CountrySortParm"] = sortOrder == "country_asc" ? "country_desc" : "country_asc";
            ViewData["PostcodeSortParm"] = sortOrder == "postcode_asc" ? "postcode_desc" : "postcode_asc";

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

            var customers = from c in context.Customers 
                            select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.FullName.ToLower().Contains(searchString.ToLower())
                                       || c.Street.ToLower().Contains(searchString.ToLower())
                                       || c.City.ToLower().Contains(searchString.ToLower())
                                       || c.State.ToLower().Contains(searchString.ToLower())
                                       || c.Country.ToLower().Contains(searchString.ToLower())
                                       || c.Postcode.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "firstname_desc":
                    customers = customers.OrderByDescending(c => c.FirstName);
                    break;
                case "lastname_asc":
                    customers = customers.OrderBy(c => c.LastName);
                    break;
                case "lastname_desc":
                    customers = customers.OrderByDescending(c => c.LastName);
                    break;
                case "address_asc":
                    customers = customers.OrderBy(c => c.Street);
                    break;
                case "address_desc":
                    customers = customers.OrderByDescending(c => c.Street);
                    break;
                case "city_asc":
                    customers = customers.OrderBy(c => c.City);
                    break;
                case "city_desc":
                    customers = customers.OrderByDescending(c => c.City);
                    break;
                case "state_asc":
                    customers = customers.OrderBy(c => c.State);
                    break;
                case "state_desc":
                    customers = customers.OrderByDescending(c => c.State);
                    break;
                case "country_asc":
                    customers = customers.OrderBy(c => c.Country);
                    break;
                case "country_desc":
                    customers = customers.OrderByDescending(c => c.Country);
                    break;
                case "postcode_asc":
                    customers = customers.OrderBy(c => c.Postcode);
                    break;
                case "postcode_desc":
                    customers = customers.OrderByDescending(c => c.Postcode);
                    break;
                default:
                    customers = customers.OrderBy(c => c.FirstName);
                    break;
            }

            int numberOfLines = 12;

            return View(await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), page ?? 1, numberOfLines));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await context.Customers
                .SingleOrDefaultAsync(m => m.ID == id);

            var orders = from o in context.Orders
                            .Where(o => o.CustomerID == id)
                            .Include(o => o.OrderItems)
                                .ThenInclude(i => i.Product)
                         select o;
            var orderList = orders.ToList();

            ViewBag.orders = orderList;

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,FirstName,LastName,Email,Gender,Street,City,State,Country,Postcode,PhoneNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                context.Add(customer);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await context.Customers.SingleOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,FirstName,LastName,Email,Gender,Street,City,State,Country,Postcode,PhoneNumber")] Customer customer)
        {
            if (id != customer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(customer);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.ID))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await context.Customers
                .SingleOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await context.Customers.SingleOrDefaultAsync(m => m.ID == id);
            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return context.Customers.Any(e => e.ID == id);
        }
    }
}