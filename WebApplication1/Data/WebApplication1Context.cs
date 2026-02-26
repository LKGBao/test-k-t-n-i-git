using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class WebApplication1Context : DbContext
    {
        public WebApplication1Context (DbContextOptions<WebApplication1Context> options)
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
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Asset_Quotation> AssetQuotations { get; set; }
        public DbSet<Asset_Quotation_Item> AssetQuotationItems { get; set; }

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

            // Asset → Asset_Category
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.Category_Id);

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

            // Asset_Quotation → Asset_Purchase_Request
            modelBuilder.Entity<Asset_Quotation>()
                .HasOne(q => q.Asset_Purchase_Request)
                .WithMany()
                .HasForeignKey(q => q.Purchase_Id);

            // Asset_Quotation → User
            modelBuilder.Entity<Asset_Quotation>()
                .HasOne(q => q.User)
                .WithMany()
                .HasForeignKey(q => q.User_Id);

            // Asset_Quotation → Supplier
            modelBuilder.Entity<Asset_Quotation>()
                .HasOne(q => q.Supplier)
                .WithMany()
                .HasForeignKey(q => q.Supplier_Id);

            // Asset_Quotation_Item → Asset_Quotation
            modelBuilder.Entity<Asset_Quotation_Item>()
                .HasOne(i => i.Asset_Quotation)
                .WithMany(q => q.Asset_Quotation_Items)
                .HasForeignKey(i => i.Request_Quotation_Id);

        }
    }

}
