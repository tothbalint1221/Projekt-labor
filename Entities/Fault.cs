using ServiceManagerApp.Entities;

namespace ServiceManagerApp;

public class Fault : BaseEntity
{
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
    public string FaultName { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Repairs { get; set; } = string.Empty;
    public Status Status { get; set;} = Status.Accepted;
}