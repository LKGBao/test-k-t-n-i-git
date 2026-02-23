using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nets_System.Models;

namespace Nets_System.Data
{
    public class Nets_SystemContext : DbContext
    {
        public Nets_SystemContext (DbContextOptions<Nets_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Branch_Manager> BranchManagers { get; set; }
        public DbSet<Assets_Purchase_Request> AssetPurchaseRequests { get; set; }
        public DbSet<Asset_Category> AssetCategories { get; set; }
        public DbSet<Asset_Purchase_Request_Item> AssetPurchaseRequestItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Branch_Manager>()
       .HasOne(bm => bm.Branch)
       .WithMany(b => b.BranchManagers)
       .HasForeignKey(bm => bm.Branch_Id);

            // BranchManager → User
            modelBuilder.Entity<Branch_Manager>()
                .HasOne(bm => bm.User)
                .WithMany(u => u.BranchManagers)
                .HasForeignKey(bm => bm.User_Id);

            // User → Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.Role_Id);

            // PurchaseRequest → BranchManager
            modelBuilder.Entity<Assets_Purchase_Request>()
                .HasOne(pr => pr.BranchManager)
                .WithMany()
                .HasForeignKey(pr => pr.Branch_Manager_Id);

            // PurchaseRequest → Items
            modelBuilder.Entity<Asset_Purchase_Request_Item>()
                .HasOne(i => i.Asset_Purchase_Request)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.Asset_Purchase_Request_Id);

            // Item → Asset_Category
            modelBuilder.Entity<Asset_Purchase_Request_Item>()
                .HasOne(i => i.Asset_Category)
                .WithMany()
                .HasForeignKey(i => i.Asset_Category_Id);
        }
    }

}
