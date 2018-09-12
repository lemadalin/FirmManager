using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirmManager.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Display(Name = "Order Number")]
        public long OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        [Required]
        public int CustomerID { get; set; }

        public virtual Customer Customer { get; set; }
        [Display(Name = "Order Items")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}