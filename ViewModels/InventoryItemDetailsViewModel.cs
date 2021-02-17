using BoardGameCompanyMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.ViewModels
{
    public class InventoryItemDetailsViewModel
    {
        public IEnumerable<BoardGame> BoardGame { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }
}
