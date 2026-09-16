namespace ServiceManagerApp;

public class Part
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public decimal PartCost { get; set; }
}