using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Models
{
    public class Headlines
    {
        public int ID { get; set; }

        [Required]
        public int BoardGameID { get; set; }

        [Display(Name = "Board Game")]
        public BoardGame BoardGame { get; set; }

        [Required]
        [Display(Name ="Image")]
        public string ImageFilePath { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Image Form File")]
        [NotMapped]
        public IFormFile FormFileUpload { get; set; }



    }
}
