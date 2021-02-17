using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class UserCartViewModel
    {
        public IList<UserCart> UserCart { get; set; }
        public int? ErrorBoardGameID { get; set; }
        public string ErrorMessage { get; set; }
        public IList<BoardGame> BoardGames { get; set; }
    }
}
