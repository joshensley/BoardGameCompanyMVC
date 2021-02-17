using BoardGameCompanyMVC.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class BoardGameDetailsViewModel
    {
        public IEnumerable<Brand> Brands { get; set; }
        public BoardGame BoardGame { get; set; }
    }
}
