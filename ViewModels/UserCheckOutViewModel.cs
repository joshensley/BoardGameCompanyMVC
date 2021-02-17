using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class UserCheckOutViewModel
    {
        public IList<UnitedStates> UnitedStates { get; set; }
        public IList<UserCart> UserCart { get; set; }
        public UserCheckOutInput UserCheckOutInput { get; set; }
        public string ApplicationUserId { get; set; }
        public bool? Error { get; set; }
        public string ErrorMessage { get; set; }
        public IList<BoardGame> BoardGames { get; set; }

    }
}
