using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Models
{
    public class InventoryItem
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "In Stock")]
        public bool InStock { get; set; }

        [Required]
        [Display(Name = "Received #")]
        public int ReceivedNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Received Date")]
        public DateTime ReceivedDate { get; set; }

        [Display(Name = "Order #")]
        public int? OrderNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ordered Date")]
        public DateTime? OrderedDate { get; set; }

        [Display(Name = "Shipping #")]
        public int? ShippingNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Shipped Date")]
        public DateTime? ShippedDate { get; set; }

        public int BoardGameID { get; set; }
        public BoardGame BoardGame { get; set; }

    }
}
