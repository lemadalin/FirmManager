using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirmManager.Data;
using FirmManager.Models;
using Microsoft.AspNetCore.Authorization;
using FirmManager.Services;

namespace FirmManager.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class OrdersController : Controller
    {
        private readonly FirmContext context;
        private readonly IMailService mailService;

        public OrdersController(FirmContext context, IMailService mailService)
        {
            this.context = context;
            this.mailService = mailService;
        }

        // GET: Orders
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["OrderNumberSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ordernumber_desc" : "";
            ViewData["OrderDateSortParm"] = sortOrder == "orderdate_asc" ? "orderdate_desc" : "orderdate_asc";
            ViewData["CustomerSortParm"] = sortOrder == "customer_asc" ? "customer_desc" : "customer_asc";
            ViewData["OrderStatusSortParm"] = sortOrder == "orderstatus_asc" ? "orderstatus_desc" : "orderstatus_asc";

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

            var orders = from o in context.Orders
                            .Include(o => o.Customer)
                            .Include(o => o.OrderItems)
                            .ThenInclude(i => i.Product)
                         select o;

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.OrderNumber.ToString().ToLower().Contains(searchString.ToLower())
                                       || o.OrderDate.ToString().ToLower().Contains(searchString.ToLower())
                                       || o.Customer.FullName.ToLower().Contains(searchString.ToLower())
                                       || o.OrderStatus.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortOrder)
            {
                case "ordernumber_desc":
                    orders = orders.OrderByDescending(o => o.OrderNumber);
                    break;
                case "orderdate_asc":
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
                case "orderdate_desc":
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
                case "customer_asc":
                    orders = orders.OrderBy(o => o.Customer.FullName);
                    break;
                case "customer_desc":
                    orders = orders.OrderByDescending(o => o.Customer.FullName);
                    break;
                case "orderstatus_asc":
                    orders = orders.OrderBy(o => o.OrderStatus);
                    break;
                case "orderstatus_desc":
                    orders = orders.OrderByDescending(o => o.OrderStatus);
                    break;
                default:
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
            }

            int numberOfLines = 12;

            return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, numberOfLines));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .SingleOrDefaultAsync(m => m.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var order = new Order();
            order.OrderItems = new List<OrderItem>();

            PopulateAssignedProducts(order);

            ViewData["CustomerID"] = new SelectList(context.Customers, "ID", "FullName");

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrderNumber,OrderStatus,OrderDate,CustomerID")] Order order, string[] selectedProducts, int[] quantity)
        {
            if (selectedProducts != null && quantity != null)
            {
                order.OrderItems = new List<OrderItem>();

                quantity = quantity.Where(val => val != 0).ToArray();

                int index = 0;
                foreach (var product in selectedProducts)
                {
                    var prod = await context.Products
                        .SingleOrDefaultAsync(i => i.ID == int.Parse(product));
                    var orderItemToAdd = new OrderItem
                    {
                        Product = prod,
                        Quantity = quantity[index]
                    };
                    order.OrderItems.Add(orderItemToAdd);
                    index++;
                }
                order.OrderDate = DateTime.Now;
                order.OrderNumber = long.Parse(order.OrderDate.Year.ToString() + order.OrderDate.Month.ToString() +
                    order.OrderDate.Day.ToString() + order.OrderDate.Hour.ToString() +
                    order.OrderDate.Minute.ToString() + order.OrderDate.Millisecond.ToString());
                order.OrderStatus = "Production";

                decimal sum = 0.0M;
                foreach (var item in order.OrderItems)
                {
                    sum += Convert.ToDecimal(item.Quantity) * item.Product.Price;
                }
                order.Total = sum;

                foreach (var item in order.OrderItems)
                {
                    var product = await context.Products
                        .SingleOrDefaultAsync(m => m.ID == item.Product.ID);
                    if (product.UnitsInStock - item.Quantity < 0)
                    {
                        ModelState.AddModelError("", "There are not enough products of type: " + product.Title);
                    }
                    else
                    {
                        product.UnitsInStock -= item.Quantity;
                        context.Products.Update(product);
                    }
                }

                var customer = await context.Customers
                    .SingleOrDefaultAsync(m => m.ID == order.CustomerID);
                mailService.SendMail(order, customer, "Production");
            }

            if (ModelState.IsValid)
            {
                context.Add(order);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateAssignedProducts(order);

            ViewData["CustomerID"] = new SelectList(context.Customers, "ID", "FullName", order.CustomerID);

            return View(order);
        }

        private void PopulateAssignedProducts(Order order)
        {
            var allProducts = context.Products;
            var viewModel = new List<Product>();

            foreach (var product in allProducts)
            {
                viewModel.Add(new Product
                {
                    Title = product.Title,
                    Category = product.Category,
                    Price = product.Price,
                    ID = product.ID,
                    Length = product.Length,
                    Width = product.Width,
                    Height = product.Height,
                    UnitsInStock = product.UnitsInStock
                });
            }

            ViewData["Products"] = viewModel;
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            PopulateAssignedProducts(order);

            ViewData["CustomerID"] = new SelectList(context.Customers, "ID", "FullName", order.CustomerID);

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrderNumber,OrderStatus,OrderDate,CustomerID")] Order order, string[] selectedProducts, int[] quantity)
        {
            if (selectedProducts != null && quantity != null)
            {
                try
                {
                    context.RemoveRange(context.OrderItems.Where(i => i.OrderID == order.ID).ToList());

                    order.OrderItems = new List<OrderItem>();

                    quantity = quantity.Where(val => val != 0).ToArray();

                    int index = 0;
                    foreach (var product in selectedProducts)
                    {
                        var prod = await context.Products
                            .SingleOrDefaultAsync(i => i.ID == int.Parse(product));
                        var orderItemToAdd = new OrderItem
                        {
                            Product = prod,
                            Quantity = quantity[index]
                        };
                        order.OrderItems.Add(orderItemToAdd);
                        index++;
                    }

                    decimal sum = 0.0M;
                    foreach (var item in order.OrderItems)
                    {
                        sum += Convert.ToDecimal(item.Quantity) * item.Product.Price;
                    }
                    order.Total = sum;

                    foreach (var item in order.OrderItems)
                    {
                        var product = await context.Products
                            .SingleOrDefaultAsync(m => m.ID == item.Product.ID);
                        if (product.UnitsInStock - item.Quantity < 0)
                        {
                            ModelState.AddModelError("", "There are not enough products of type: " + product.Title);
                        }
                        else
                        {
                            product.UnitsInStock -= item.Quantity;
                            context.Products.Update(product);
                        }
                    }

                    var customer = await context.Customers
                        .SingleOrDefaultAsync(m => m.ID == order.CustomerID);
                    mailService.SendMail(order, customer, order.OrderStatus);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                context.Update(order);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateAssignedProducts(order);

            ViewData["CustomerID"] = new SelectList(context.Customers, "ID", "FullName", order.CustomerID);

            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await context.Orders
                 .Include(o => o.Customer)
                 .Include(o => o.OrderItems)
                     .ThenInclude(i => i.Product)
                 .SingleOrDefaultAsync(m => m.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await context.Orders.SingleOrDefaultAsync(m => m.ID == id);
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return context.Orders.Any(e => e.ID == id);
        }
    }
}