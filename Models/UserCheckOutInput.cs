using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Models
{
    public class UserCheckOutInput
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Order #")]
        public int OrderNumber { get; set; }

        // Links
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [NotMapped]
        public IList<UserCart> UserCart { get; set; }

        // Board Game Cost
        [Required]
        public decimal SubTotal { get; set; }

        [Required]
        public decimal Tax { get; set; }

        [Required]
        public decimal Shipping { get; set; }

        [Required]
        public decimal GrandTotal { get; set; }

        // Billing Address
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First name")]
        public string FirstNameBillingAddress { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last name")]
        public string LastNameBillingAddress { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string AddressBillingAddress { get; set; }

        [Display(Name = "Address 2 (Optional)")]
        public string Address2BillingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string CityBillingAddress { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Country")]
        public string CountryBillingAddress { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public string StateBillingAddress { get; set; }

        [Required(ErrorMessage = "Postal code")]
        [Display(Name = "Postal Code")]
        public string PostalCodeBillingAddress { get; set; }


        // Shipping Address
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First name")]
        public string FirstNameShippingAddress { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last name")]
        public string LastNameShippingAddress { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string AddressShippingAddress { get; set; }

        [Display(Name = "Address 2 (Optional)")]
        public string Address2ShippingAddress { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Country")]
        public string CountryShippingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string CityShippingAddress { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public string StateShippingAddress { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [Display(Name = "Postal code")]
        public string PostalCodeShippingAddress { get; set; }

        [Display(Name = "Transaction Id")]
        public string BalanceTransactionId { get; set; }

    }
}
