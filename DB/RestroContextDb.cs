using Microsoft.EntityFrameworkCore;
using RestroManagementSystem.Models;


namespace RestroManagementSystem.DB
{
    public class RestroContextDb:DbContext
    {
        
        public RestroContextDb(DbContextOptions<RestroContextDb> options) : base(options) { }

        //table representations
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Orderdish> Ordersdishes { get; set; }
        public DbSet<Dish> dishes { get; set; }

    }
}
