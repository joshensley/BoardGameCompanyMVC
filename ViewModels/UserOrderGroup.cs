using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class UserOrderGroup
    {


        [Display(Name = "Order #")]
        public int OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Orders")]
        public int OrderCount { get; set; }

        [Display(Name = "Board Game ID")]
        public int BoardGameID { get; set; }

        [Display(Name = "Title")]
        public string BoardGameTitle { get; set; }

        [Display(Name = "Image File Path")]
        public string BoardGameImageFilePath { get; set; }
    }
}
