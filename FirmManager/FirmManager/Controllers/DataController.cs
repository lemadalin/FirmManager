using FirmManager.Data;
using FirmManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FirmManager.Controllers
{
    [Route("api/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class DataController : ControllerBase
    {
        private readonly FirmContext context;
        private readonly IHostingEnvironment environment;

        public DataController(FirmContext context, IHostingEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        [Route("customersPerState")]
        public string CustomersPerState()
        {
            var customersJson = CustomersJson();
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(customersJson);

            var customersPerStateMap = new Dictionary<string, string>();

            foreach (Customer c in customers)
            {
                var abbreviatedState = AbbreviateState(c.State);
                if (!customersPerStateMap.ContainsKey(abbreviatedState))
                {
                    customersPerStateMap.Add(abbreviatedState, "1");
                }
                else
                {
                    customersPerStateMap[abbreviatedState] = Int32.Parse(customersPerStateMap[abbreviatedState]) + 1 + "";
                }
            }

            var customersPerStateTsvPath = Path.Combine(environment.ContentRootPath, "Data/Mock/customersPerState.tsv");
            System.IO.File.WriteAllText(customersPerStateTsvPath, string.Empty);
            TextWriter tw = new StreamWriter(customersPerStateTsvPath, true);
            tw.WriteLine("State\tCount");

            foreach (KeyValuePair<string, string> kvp in customersPerStateMap)
            {
                tw.WriteLine(kvp.Key + "\t" + kvp.Value);
            }

            tw.Close();

            var customersPerStateTsv = System.IO.File.ReadAllText(customersPerStateTsvPath);

            return customersPerStateTsv;
        }

        [Route("ordersPerMonth")]
        public string OrdersPerMonth()
        {
            var ordersJson = OrdersJson();
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersJson);

            var ordersPerMonthMap = new SortedDictionary<DateTime, string>();

            foreach (var o in orders)
            {
                var orderDate = o.OrderDate;
                var orderMonth = orderDate.ToString("MMM yyyy");
                var orderYear = orderDate.Year;

                DateTime orderMonthDt = DateTime.ParseExact(orderMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

                if (orderYear >= 2017)
                {
                    if (!ordersPerMonthMap.ContainsKey(orderMonthDt))
                    {
                        ordersPerMonthMap.Add(orderMonthDt, "1");
                    }
                    else
                    {
                        ordersPerMonthMap[orderMonthDt] = Int32.Parse(ordersPerMonthMap[orderMonthDt]) + 1 + "";
                    }
                }
            }

            var ordersPerMonthTsvPath = Path.Combine(environment.ContentRootPath, "Data/Mock/ordersPerMonth.tsv");
            System.IO.File.WriteAllText(ordersPerMonthTsvPath, string.Empty);
            TextWriter tw = new StreamWriter(ordersPerMonthTsvPath, true);
            tw.WriteLine("Month\tCount");

            foreach (KeyValuePair<DateTime, string> kvp in ordersPerMonthMap.OrderBy(k => k.Key))
            {
                tw.WriteLine(kvp.Key.ToString("MMM") + " " + kvp.Key.Year + "\t" + kvp.Value);
            }

            tw.Close();

            var ordersPerMonthTsv = System.IO.File.ReadAllText(ordersPerMonthTsvPath);

            return ordersPerMonthTsv;
        }

        [Route("productsSoldPerCategory")]
        public string ProductsSoldPerCategory()
        {
            var ordersJson = OrdersJson();
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersJson);

            var productsSoldPerCategoryMap = new Dictionary<string, int>();

            var products = context.Products.ToList();

            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    var productID = item.ProductID;
                    var product = products.Where(p => p.ID == productID).Single();
                    var productCategory = product.Category;

                    if (!productsSoldPerCategoryMap.ContainsKey(productCategory))
                    {
                        productsSoldPerCategoryMap.Add(productCategory, item.Quantity);
                    }
                    else
                    {
                        productsSoldPerCategoryMap[productCategory] = productsSoldPerCategoryMap[productCategory] + item.Quantity;
                    }
                }
            }

            var productsSoldPerCategoryTsvPath = Path.Combine(environment.ContentRootPath, "Data/Mock/productsSoldPerCategory.tsv");
            System.IO.File.WriteAllText(productsSoldPerCategoryTsvPath, string.Empty);
            TextWriter tw = new StreamWriter(productsSoldPerCategoryTsvPath, true);
            tw.WriteLine("Category\tCount");

            foreach (KeyValuePair<string, int> kvp in productsSoldPerCategoryMap)
            {
                tw.WriteLine(kvp.Key + "\t" + kvp.Value);
            }

            tw.Close();

            var productsSoldPerCategoryTsv = System.IO.File.ReadAllText(productsSoldPerCategoryTsvPath);

            return productsSoldPerCategoryTsv;
        }

        [Route("totalSalesPerCategory")]
        public string TotalSalesPerCategory()
        {
            var ordersJson = OrdersJson();
            var orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(ordersJson);

            var totalSalesPerCategoryMap = new Dictionary<string, decimal>();

            var products = context.Products.ToList();

            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    var productID = item.ProductID;
                    var product = products.Where(p => p.ID == productID).Single();
                    var productCategory = product.Category;
                    var productPrice = product.Price;

                    if (!totalSalesPerCategoryMap.ContainsKey(productCategory))
                    {
                        totalSalesPerCategoryMap.Add(productCategory, item.Quantity * productPrice);
                    }
                    else
                    {
                        totalSalesPerCategoryMap[productCategory] = totalSalesPerCategoryMap[productCategory] + item.Quantity * productPrice;
                    }
                }
            }

            var totalSalesPerCategoryTsvPath = Path.Combine(environment.ContentRootPath, "Data/Mock/totalSalesPerCategory.tsv");
            System.IO.File.WriteAllText(totalSalesPerCategoryTsvPath, string.Empty);
            TextWriter tw = new StreamWriter(totalSalesPerCategoryTsvPath, true);
            tw.WriteLine("Category\tSales");

            foreach (KeyValuePair<string, decimal> kvp in totalSalesPerCategoryMap)
            {
                tw.WriteLine(kvp.Key + "\t" + kvp.Value);
            }

            tw.Close();

            var totalSalesPerCategoryTsv = System.IO.File.ReadAllText(totalSalesPerCategoryTsvPath);

            return totalSalesPerCategoryTsv;
        }

        [Route("customersJson")]
        public string CustomersJson()
        {
            var customers = context.Customers.ToList();
            var customersJson = JsonConvert.SerializeObject(customers);

            return customersJson;
        }

        [Route("productsJson")]
        public string ProductsJson()
        {
            var products = context.Products.ToList();
            var productsJson = JsonConvert.SerializeObject(products);

            return productsJson;
        }

        [Route("ordersJson")]
        public string OrdersJson()
        {
            var orders = context.Orders
                            .Include(o => o.Customer)
                            .Include(o => o.OrderItems)
                                .ThenInclude(i => i.Product)
                            .ToList();

            var ordersJson = JsonConvert.SerializeObject(orders, Formatting.None, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }); 

            return ordersJson;
        }

        private string AbbreviateState(string state)
        {
            string[,] states = new string[51, 2] {
                {"Alabama", "AL"},
                {"Alaska", "AK"},
                {"Arizona", "AZ"},
                {"Arkansas", "AR"},
                {"California", "CA"},
                {"Colorado", "CO"},
                {"Connecticut", "CT"},
                {"Delaware", "DE"},
                {"District of Columbia", "DC" },
                {"Florida", "FL"},
                {"Georgia", "GA"},
                {"Hawaii", "HI"},
                {"Idaho", "ID"},
                {"Illinois", "IL"},
                {"Indiana", "IN"},
                {"Iowa", "IA"},
                {"Kansas", "KS"},
                {"Kentucky", "KY"},
                {"Louisiana", "LA"},
                {"Maine", "ME"},
                {"Maryland", "MD"},
                {"Massachusetts", "MA"},
                {"Michigan", "MI"},
                {"Minnesota", "MN"},
                {"Mississippi", "MS"},
                {"Missouri", "MO"},
                {"Montana", "MT"},
                {"Nebraska", "NE"},
                {"Nevada", "NV"},
                {"New Hampshire", "NH"},
                {"New Jersey", "NJ"},
                {"New Mexico", "NM"},
                {"New York", "NY"},
                {"North Carolina", "NC"},
                {"North Dakota", "ND"},
                {"Ohio", "OH"},
                {"Oklahoma", "OK"},
                {"Oregon", "OR"},
                {"Pennsylvania", "PA"},
                {"Rhode Island", "RI"},
                {"South Carolina", "SC"},
                {"South Dakota", "SD"},
                {"Tennessee", "TN"},
                {"Texas", "TX"},
                {"Utah", "UT"},
                {"Vermont", "VT"},
                {"Virginia", "VA"},
                {"Washington", "WA"},
                {"West Virginia", "WV"},
                {"Wisconsin", "WI"},
                {"Wyoming", "WY"},
            };

            int index = 0;
            bool flag = false;
            for (var i = 0; i < 51; i++)
            {
                if (states[i, 0] == state)
                {
                    index = i;
                    flag = true;
                    break;
                }
            }

            if (flag == true)
            {
                return states[index, 1];
            }
            else
            {
                return state;
            }
        }
    }
}