using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Models
{
    public class UserBoardGameReview
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Review")]
        public string UserReview { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime UserReviewDate { get; set; }

        [Required]
        [Display(Name = "User Id")]
        public string UserIDReview { get; set; }

        public int BoardGameID { get; set; }
        public BoardGame BoardGame { get; set; }
    }
}
