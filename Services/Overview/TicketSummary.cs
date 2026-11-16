using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Services.Overview
{
    public class TicketSummary
    {
        public int Id { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerPhone { get; init; } = string.Empty;
        public string EquipmentName { get; init; } = string.Empty;
        public string SerialNumber {  get; init; } = string.Empty;
        public Status Status {  get; init; }
        public DateTime IntakeDate { get; init; }
        public int? UserId { get; init; }
        public string? AssignedUserName {  get; init; }
        
    }
}
