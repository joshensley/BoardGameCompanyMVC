using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class HeadlinesViewModel
    {
        public Headlines Headline { get; set; }
        public IEnumerable<BoardGame> BoardGames { get; set; }
    }
}
