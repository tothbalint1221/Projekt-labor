using ServiceManagerApp.Entities;

namespace ServiceManagerApp;

public class ServiceTicket : BaseEntity
{
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
    public DateTime IntakeDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public Status Status { get; set;} = Status.Accepted;
    public decimal LaborCost { get; set; }
    public decimal Price { get; set; }
}