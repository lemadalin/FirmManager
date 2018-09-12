using System.ComponentModel.DataAnnotations;

namespace FirmManager.Models
{
    public class OrderItem
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Quantity cannot be longer than 50 characters.")]
        public int Quantity { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int OrderID { get; set; }

        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}