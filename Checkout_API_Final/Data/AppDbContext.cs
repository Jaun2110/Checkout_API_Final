using Microsoft.EntityFrameworkCore;
using Checkout_API_Final.Models;
namespace Checkout_API_Final.Data;



public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<CheckoutItem> CheckoutItems { get; set; }


}
