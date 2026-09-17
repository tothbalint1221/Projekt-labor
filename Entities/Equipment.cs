using ServiceManagerApp.Entities;

namespace ServiceManagerApp;

public class Equipment : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public List<Fault> Faults { get; set; } = new();
}