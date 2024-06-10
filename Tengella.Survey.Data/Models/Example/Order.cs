namespace Tengella.Survey.Data.Models.Example;

public class Order
{
    public Order()
    {
        OrderDetails = new HashSet<OrderDetail>();
    }

    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }

    // Foreign key for Customer
    public int CustomerId { get; set; }

    // Navigation Properties
    public Customer? Customer { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; }
}
