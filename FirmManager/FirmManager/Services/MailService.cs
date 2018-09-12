using FirmManager.Models;
using System.Net;
using System.Net.Mail;

namespace FirmManager.Services
{
    public class MailService : IMailService
    {
        public void SendMail(Order order, Customer customer, string code)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("furniture.express12@gmail.com");
            mailMessage.To.Add(customer.Email);
            mailMessage.Subject = "Furniture Express - Order number #" + order.OrderNumber;

            switch (code)
            {
                case "Production":
                    mailMessage.Body = "ORDER IN PRODUCTION\r\n\r\n" +
                        "Dear " + customer.FullName + ",\r\n" +
                        "We want to let you know that your order #" + order.OrderNumber + ", processed on " + order.OrderDate.ToString() + ", is now in production.\r\n" +
                        "You can find more details about your order below.\r\n" +
                        "Thank you for choosing us!\r\n\r\n" +
                        "Shipping address: " + customer.Street + " " + customer.City + " " + customer.State + " " + customer.Country + "\r\n" +
                        "Items: ";
                    foreach (var item in order.OrderItems)
                    {
                        mailMessage.Body += item.Product.Title + " - Quantity: " + item.Quantity + "\r\n";
                    }
                    mailMessage.Body += "Total: $" + order.Total;
                    break;
                case "Shipped":
                    mailMessage.Body = "ORDER SHIPPED\r\n\r\n" +
                        "Dear " + customer.FullName + ",\r\n" +
                        "We want to let you know that your order #" + order.OrderNumber + ", processed on " + order.OrderDate.ToString() + ", has been shipped.\r\n" +
                        "You can find more details about your order below.\r\n" +
                        "Thank you for choosing us!\r\n\r\n" +
                        "Shipping address: " + customer.Street + " " + customer.City + " " + customer.State + " " + customer.Country + "\r\n" +
                        "Items: ";
                    foreach (var item in order.OrderItems)
                    {
                        mailMessage.Body += item.Product.Title + " - Quantity: " + item.Quantity + "\r\n";
                    }
                    mailMessage.Body += "Total: $" + order.Total;
                    break;
                case "Canceled":
                    mailMessage.Body = "ORDER CANCELED\r\n\r\n" +
                        "Dear " + customer.FullName + ",\r\n" +
                        "We want to let you know that your order #" + order.OrderNumber + ", processed on " + order.OrderDate.ToString() + ", has been canceled.\r\n" +
                        "If you think that there has been a mistake, please contact us.\r\n" +
                        "Thank you for choosing us!";
                    mailMessage.Body += "Total: $" + order.Total;
                    break;
            }

            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;

                // Password got hidden for obvious reasons
                smtpClient.Credentials = new NetworkCredential("furniture.express12", "*****");

                smtpClient.Send(mailMessage.From.ToString(), mailMessage.To.ToString(), mailMessage.Subject, mailMessage.Body);
            }
        }
    }
}