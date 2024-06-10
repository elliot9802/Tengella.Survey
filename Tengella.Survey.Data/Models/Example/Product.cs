namespace Tengella.Survey.Data.Models.Example;

public class Product
{
    public Product()
    {
        OrderDetails = new HashSet<OrderDetail>();
    }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Navigation Property
    public ICollection<OrderDetail> OrderDetails { get; set; }
}
