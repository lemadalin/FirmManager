using System.ComponentModel.DataAnnotations;

namespace FirmManager.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Title cannot be longer than 30 characters.")]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int Length { get; set; }

        [Required]
        public int Width { get; set; }

        [Required]
        public int Height { get; set; }

        public string Size => Width + " x " + Length + " x " + Height + " cm";

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Units In Stock")]
        public int UnitsInStock { get; set; }
    }
}