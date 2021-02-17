using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Models
{
    public class UserCart
    {
        public int ID { get; set; }

        [Required]
        public int BoardGameID { get; set; }
        public BoardGame BoardGame { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        [Range(1, Int32.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
