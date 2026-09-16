namespace ServiceManagerApp;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public List<Equipment> Equipments { get; set; } = new();
}