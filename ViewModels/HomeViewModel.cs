using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class HomeViewModel
    {
        public PaginatedList<BoardGame> BoardGames{ get; set; }
        public UserCart UserCart { get; set; }
        public IList<Headlines> Headlines { get; set; }
        public string UserId { get; set; }

    }
}
