using Microsoft.EntityFrameworkCore;
using T2305MPK3.Models;

namespace T2305MPK3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<LoginMaster> LoginMasters { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Caterer> Caterers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Sizes> Sizes { get; set; }
        public DbSet<ItemVariants> ItemVariants { get; set; }
        public DbSet<CustOrder> CustOrders { get; set; }
        public DbSet<CustOrderDetail> CustOrderDetails { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
    }
}
