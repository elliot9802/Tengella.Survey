using Tengella.Survey.Data.Models.Example;
using Microsoft.EntityFrameworkCore;

namespace Tengella.Survey.Data;

public class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seeding a customer
        modelBuilder.Entity<Customer>().HasData(new Customer
        {
            CustomerId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        });

        // Seeding products
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, ProductName = "Product 1", Price = 100.0m },
            new Product { ProductId = 2, ProductName = "Product 2", Price = 200.0m },
            new Product { ProductId = 3, ProductName = "Product 3", Price = 300.0m }
        );

        // Seeding an order
        modelBuilder.Entity<Order>().HasData(new Order
        {
            OrderId = 1,
            OrderDate = DateTime.Now,
            CustomerId = 1 // Assuming the customer with ID 1 exists
        });

        // Seeding order details
        modelBuilder.Entity<OrderDetail>().HasData(
            new OrderDetail { OrderDetailId = 1, OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrderDetail { OrderDetailId = 2, OrderId = 1, ProductId = 2, Quantity = 1 },
            new OrderDetail { OrderDetailId = 3, OrderId = 1, ProductId = 3, Quantity = 3 }
        );
    }
}
