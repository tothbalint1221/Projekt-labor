using System;
using System.Collections.Generic;
using System.Text;
using ServiceManagerApp.Repositories;
using ServiceManagerApp.Services.Users;


namespace ServiceManagerApp.Services.Overview
{
    public class OverviewService : IOverviewService
    {
        private readonly ServiceTicketRepository ticketRepository;

        public OverviewService(ServiceTicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public async Task<OverviewData> GetOverviewAsync(SignedInUser? user)
        {
            if(user is null)
            {
                return new OverviewData();
            }

            bool showAll = user.Role != Role.Technician;
            int? userId = showAll ? null : user.Id;

            var tickets = await ticketRepository.GetOpenTicketsAsync(userId);
            var summeries = tickets.Select(ToSummary).ToList();

            return new OverviewData
            {
                IsSignedIn = true,
                ShowAllTickets = showAll,
                AcceptedCount = summeries.Count(t => t.Status == Status.Accepted),
                DiagnosingCount = summeries.Count(t => t.Status == Status.Diagnosing),
                InRepairCount = summeries.Count(t => t.Status == Status.InRepair),
                FinishedCount = summeries.Count(t => t.Status == Status.Finished),
                ActiveTickets = summeries.Where(t => t.Status != Status.Finished).ToList(),
                ReadyTickets = summeries.Where(t => t.Status == Status.Finished).ToList()

            };

        }

        private static TicketSummary ToSummary(ServiceTicket ticket) => new()
        {
            Id = ticket.Id,
            CustomerName = ticket.Equipment.Customer.Name,
            CustomerPhone = ticket.Equipment.Customer.PhoneNumber,
            EquipmentName = $"{ticket.Equipment.Brand} {ticket.Equipment.Model}",
            SerialNumber = ticket.Equipment.SerialNumber,
            Status = ticket.Status,
            IntakeDate = ticket.IntakeDate,
            UserId = ticket.UserId,
            AssignedUserName = ticket.User?.Name
        };
    }
}
