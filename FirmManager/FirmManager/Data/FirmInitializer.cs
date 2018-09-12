using FirmManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FirmManager.Data
{
    public class FirmInitializer
    {
        private readonly FirmContext context;
        private readonly IHostingEnvironment environment;
        private readonly UserManager<User> userManager;

        public FirmInitializer(
            FirmContext context,
            IHostingEnvironment environment,
            UserManager<User> userManager)
        {
            this.context = context;
            this.environment = environment;
            this.userManager = userManager;
        }

        public async Task Seed()
        {
            context.Database.EnsureCreated();

            var user = await userManager.FindByEmailAsync("furniture.express12@gmail.com");

            if (user == null)
            {
                user = new User()
                {
                    UserName = "furniture.express12@gmail.com",
                    Email = "furniture.express12@gmail.com"
                };

                var result = await userManager.CreateAsync(user, "P@ssw0rd!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create default user!");
                }
            }

            if (context.Customers.Any())
            {
                return;
            }

            var customersJsonPath = Path.Combine(environment.ContentRootPath, "Data/Mock/customers.json");
            var customersJson = File.ReadAllText(customersJsonPath);
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(customersJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            var productsJsonPath = Path.Combine(environment.ContentRootPath, "Data/Mock/products.json");
            var productsJson = File.ReadAllText(productsJsonPath);
            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(productsJson);

            context.Products.AddRange(products);
            context.SaveChanges();

            var ordersJsonPath = Path.Combine(environment.ContentRootPath, "Data/Mock/orders.json");
            var ordersJson = File.ReadAllText(ordersJsonPath);
            var format = "yyyy-MM-dd HH:mm:ss";
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersJson, dateTimeConverter);

            context.Orders.AddRange(orders);
            context.SaveChanges();

            var newOrders = context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product).ToList();

            foreach (var order in newOrders)
            {
                decimal sum = 0.0M;
                foreach (var item in order.OrderItems)
                {
                    sum += Convert.ToDecimal(item.Quantity) * item.Product.Price;
                }
                order.Total = sum;
            }

            context.Orders.UpdateRange(newOrders);
            context.SaveChanges();
        }
    }
}