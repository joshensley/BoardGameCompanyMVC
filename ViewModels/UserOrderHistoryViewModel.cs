using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class UserOrderHistoryViewModel
    {
        public IList<InventoryItem> InventoryItems { get; set; }
        public IList<UserCheckOutInput> UserCheckOutInput { get; set; }
    }
}
