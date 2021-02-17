using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class UserOrderConfirmationViewModel
    {
        public IList<UserOrder> UserOrder { get; set; }
        public UserCheckOutInput UserCheckOutInput { get; set; }
    }
}
