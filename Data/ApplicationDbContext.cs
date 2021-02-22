using BoardGameCompanyMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameCompanyMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserBoardGameReview> UserBoardGameReviews { get; set; }
        public DbSet<UserCart> UserCart { get; set; }
        public DbSet<UserCheckOutInput> UserCheckOutInputs { get; set; }
        public DbSet<UserOrder> UserOrders { get; set; }
        public DbSet<Headlines> Headlines { get; set; }
        public DbSet<UnitedStates> UnitedStates { get; set; }

    }
}
