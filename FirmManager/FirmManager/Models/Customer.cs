using System.ComponentModel.DataAnnotations;

namespace FirmManager.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => FirstName + " " + LastName;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "Email address cannot be longer than 50 characters.")]
        [Display(Name = "Email Street")]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Gender cannot be longer than 15 characters.")]
        public string Gender { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Street cannot be longer than 100 characters.")]
        public string Street { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters.")]
        public string City { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters.")]
        public string State { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Country cannot be longer than 50 characters.")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        [StringLength(5, ErrorMessage = "Postcode must be exact 5 digits.", MinimumLength = 5)]
        public string Postcode { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"\d{3}[-]\d{3}[-]\d{4}", ErrorMessage = "Incorrect phone number format! It should be: xxx-xxx-xxxx")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}