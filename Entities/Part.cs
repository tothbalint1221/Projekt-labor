using ServiceManagerApp.Entities;

namespace ServiceManagerApp;

public class Part : BaseEntity
{
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public decimal PartCost { get; set; }
}