namespace Tengella.Survey.Data.Models.Example;

public class Customer
{
    public Customer()
    {
        Orders = new HashSet<Order>();
    }

    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<Order> Orders { get; set; }
}
