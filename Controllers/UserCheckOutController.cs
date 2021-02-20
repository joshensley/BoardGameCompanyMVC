using BoardGameCompanyMVC.Data;
using BoardGameCompanyMVC.Models;
using BoardGameCompanyMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompanyMVC.Controllers
{
    [Authorize(Roles = "Admin,Customer")]
    public class UserCheckOutController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;

        public UserCheckOutController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
            List<int> errorBoardGameIds,
            string errorMessage,
            bool? error = false)
        {
            // If there are any errors with quantity a list of board games with errors will be constructed
            // then displayed to the user on the check out page
            IList<BoardGame> boardGames = new List<BoardGame>();
            if ((errorBoardGameIds.Count() > 0) && (error == true))
            {
                foreach (var item in errorBoardGameIds)
                {
                    BoardGame boardGame = await _db.BoardGames
                        .Include(i => i.InventoryItems.Where(i => i.InStock == true))
                        .FirstOrDefaultAsync(i => i.ID == item);

                    boardGames.Add(boardGame);
                }
            };

            // Get user information
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            // User Cart
            var userCart = await _db.UserCart
                .Where(u => u.ApplicationUserId == user.Id)
                .Include(u => u.BoardGame)
                .ToListAsync();

            // Cost
            decimal subtotal = Math.Round(userCart.Sum(u => (u.BoardGame.Price * u.Quantity)), 2);
            decimal tax = Math.Round((subtotal * 0.0625m), 2);
            decimal shipping = Math.Round(5.00m, 2);
            decimal grandTotal = subtotal + tax + shipping;

            // States
            var unitedStates = await _db.UnitedStates
                .OrderBy(u => u.State)
                .ToListAsync();

            UserCheckOutInput UserCheckOutInput = new UserCheckOutInput()
            {
                SubTotal = subtotal,
                Tax = tax,
                Shipping = shipping,
                GrandTotal = grandTotal
            };


            UserCheckOutViewModel UserCheckOutViewModel = new UserCheckOutViewModel()
            {
                UnitedStates = unitedStates,
                UserCart = userCart,
                UserCheckOutInput = UserCheckOutInput,
                ApplicationUserId = user.Id,
                Error = error,
                BoardGames = boardGames,
                ErrorMessage = errorMessage
            };

            return View(UserCheckOutViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("OrderNumber", "ApplicationUserId", "SubTotal","Tax", "Shipping", "GrandTotal", 
            "FirstNameBillingAddress", "LastNameBillingAddress", "AddressBillingAddress", 
            "Address2BillingAddress", "CityBillingAddress", "CountryBillingAddress", 
            "StateBillingAddress", "PostalCodeBillingAddress", "FirstNameShippingAddress", 
            "LastNameShippingAddress", "AddressShippingAddress", "Address2ShippingAddress", 
            "CountryShippingAddress", "CityShippingAddress", "StateShippingAddress", 
            "PostalCodeShippingAddress", "NameOnCard", "CreditCardNumber", 
            "Expiration", "CVV", "UserCart")] UserCheckOutInput userCheckOutInput,
            string stripeEmail,
            string stripeToken)
        {
            string balanceTransactionId = "";

            // Check if enough inventory items exist in the database to fulfill the user's order
            // If not return and redirect the error
            List<int> boardGameIdsList = new List<int>();
            foreach (var item in userCheckOutInput.UserCart)
            {
                var inventoryItemCount = _db.InventoryItems
                    .Where(i => i.BoardGameID == item.BoardGameID)
                    .Where(i => i.InStock)
                    .Count();

                if (item.Quantity > inventoryItemCount)
                {
                    boardGameIdsList.Add(item.BoardGameID);
                }
            }

            if (boardGameIdsList.Count() > 0)
            {
                return RedirectToAction("Index",
                    new
                    {
                        errorBoardGameIds = boardGameIdsList,
                        error = true
                    });
            }

            // Random "Order Number" generated
            var rand = new Random();
            var uid = rand.Next(100000, 1000000);

            // Process payment
            try
            {
                var customers = new CustomerService();
                var charges = new ChargeService();

                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    Source = stripeToken
                });

                long totalAmount = Convert.ToInt64(userCheckOutInput.GrandTotal * 100);

                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = totalAmount,
                    Description = "Board Game Payment",
                    Currency = "usd",
                    Customer = customer.Id,
                    ReceiptEmail = stripeEmail,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "OrderNumber", uid.ToString() }
                    }
                });

                if (charge.Status == "succeeded")
                {
                    balanceTransactionId = charge.BalanceTransactionId;
                }
                else
                {
                    return RedirectToAction("Index", new { error = true, errorMessage = charge.FailureMessage });
                }
            }
            catch (StripeException e)
            {
                switch (e.StripeError.ErrorDescription)
                {
                    default:
                        return RedirectToAction("Index", new { error = true, errorMessage = e.StripeError.Message } );
                }
            }

            // Update the Inventory in database
            // Cycle through items in cart
            foreach (var item in userCheckOutInput.UserCart)
            {
                // Cycle through quantity of each item
                for (int i = 0; i < item.Quantity ; i++)
                {
                    var inventoryItemToUpdate = await _db.InventoryItems
                        .Where(i => i.BoardGameID == item.BoardGameID)
                        .Where(i => i.InStock == true)
                        .OrderByDescending(i => i.ReceivedDate)
                        .FirstOrDefaultAsync();

                    await TryUpdateModelAsync<InventoryItem>(
                        inventoryItemToUpdate,
                        "",
                        i => i.InStock,
                        i => i.OrderedDate,
                        i => i.OrderNumber);
                    {
                        inventoryItemToUpdate.OrderNumber = uid;
                        inventoryItemToUpdate.OrderedDate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                        inventoryItemToUpdate.InStock = false;
                        await _db.SaveChangesAsync();
                    }
                }
            }

            // Add Order Information
            var emptyUserCheckOutInput = new UserCheckOutInput();
            if (ModelState.IsValid)
            {
                await TryUpdateModelAsync<UserCheckOutInput>(
                    emptyUserCheckOutInput,
                    "usercheckoutinput",
                    i => i.OrderNumber,
                    i => i.ApplicationUserId,
                    i => i.SubTotal,
                    i => i.Tax,
                    i => i.Shipping,
                    i => i.GrandTotal,
                    i => i.FirstNameBillingAddress,
                    i => i.LastNameBillingAddress,
                    i => i.AddressBillingAddress,
                    i => i.Address2BillingAddress,
                    i => i.CityBillingAddress,
                    i => i.CountryBillingAddress,
                    i => i.StateBillingAddress,
                    i => i.PostalCodeBillingAddress,
                    i => i.FirstNameShippingAddress,
                    i => i.LastNameShippingAddress,
                    i => i.AddressShippingAddress,
                    i => i.Address2ShippingAddress,
                    i => i.CountryShippingAddress,
                    i => i.CityShippingAddress,
                    i => i.StateShippingAddress,
                    i => i.PostalCodeShippingAddress);
                {
                    emptyUserCheckOutInput.OrderNumber = uid;
                    emptyUserCheckOutInput.BalanceTransactionId = balanceTransactionId;
                    _db.UserCheckOutInputs.Add(emptyUserCheckOutInput);
                    await _db.SaveChangesAsync();
                }
            }

            
            // Add User Orders
            foreach (var item in userCheckOutInput.UserCart)
            {
                var boardGame = await _db.BoardGames.FindAsync(item.BoardGameID);

                var emptyUserOrder = new UserOrder();

                await TryUpdateModelAsync<UserOrder>(
                    emptyUserOrder,
                    "",
                    i => i.BoardGameID,
                    i => i.OrderNumber,
                    i => i.Quantity,
                    i => i.ApplicationUserId
                    );
                {
                    emptyUserOrder.BoardGameID = item.BoardGameID;
                    emptyUserOrder.OrderNumber = uid;
                    emptyUserOrder.Quantity = item.Quantity;
                    emptyUserOrder.ApplicationUserId = item.ApplicationUserId;
                    emptyUserOrder.PurchasedPrice = boardGame.Price;
                    _db.UserOrders.Add(emptyUserOrder);
                    await _db.SaveChangesAsync();
                }
            }

            // Delete all items from cart
            foreach (var item in userCheckOutInput.UserCart)
            {
                var deleteItem = await _db.UserCart.FindAsync(item.ID);

                if (deleteItem != null)
                {
                    _db.UserCart.Remove(deleteItem);
                    await _db.SaveChangesAsync();
                }
            }


            // Redirect to a confirmation page showing details of the order
            return RedirectToAction("Index", "UserOrderConfirmation", new { orderNumber = uid });
        }

    }
}
