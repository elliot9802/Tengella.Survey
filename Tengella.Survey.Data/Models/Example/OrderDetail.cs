namespace Tengella.Survey.Data.Models.Example;

public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int Quantity { get; set; }

    // Foreign keys
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    // Navigation Properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
