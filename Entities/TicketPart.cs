using ServiceManagerApp.Entities;

namespace ServiceManagerApp;

public class TicketPart : BaseEntity
{
    public int ServiceTicketId { get; set; }
    public ServiceTicket ServiceTicket { get; set; } = null!;
    public int PartId { get; set; }
    public Part Part { get; set; } = null!;
    public int Quantity { get; set; } = 1;
}