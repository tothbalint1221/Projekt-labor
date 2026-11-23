using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ServiceManagerApp.Repositories
{
    public class ServiceTicketRepository(AppDbContext context) : BaseRepository<ServiceTicket>(context)
    {
        public override async Task<List<ServiceTicket>> GetAllAsync()
        {
            return await DbSet
                .Include(st => st.User)
                .Include(st => st.Equipment)
                    .ThenInclude(equipment => equipment.Customer)
                .Include(st => st.Faults)
                .ToListAsync();
        }

        public async Task<List<ServiceTicket>> GetOpenTicketsAsync(int? userId)
        {
            var query = DbSet
                .AsNoTracking()
                .Include(st => st.User)
                .Include(st => st.Equipment)
                    .ThenInclude(e => e.Customer)
                .Where(st => st.Status == Status.Accepted 
                          || st.Status == Status.Diagnosing
                          || st.Status == Status.InRepair
                          || st.Status == Status.Finished
                          );

            if(userId is not null)
            {
                query = query.Where(st => st.UserId == userId);
            }

            return await query
                .OrderBy(st => st.IntakeDate)
                .ToListAsync();

        }


        public async Task UpdateStatusAsync(ServiceTicket ticket, Status status, string changeLogEntry)
        {
            var trackedTicket = await DbSet.SingleOrDefaultAsync(st => st.Id == ticket.Id);
            if (trackedTicket is null)
            {
                throw new InvalidOperationException($"Service ticket {ticket.Id} could not be found.");
            }

            trackedTicket.Status = status;
            trackedTicket.ChangeLog.Add(changeLogEntry);
            if (!ReferenceEquals(ticket, trackedTicket))
            {
                ticket.ChangeLog.Add(changeLogEntry);
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateLaborCostAsync(ServiceTicket ticket, decimal laborCost)
        {
            var trackedTicket = await DbSet.SingleOrDefaultAsync(st => st.Id == ticket.Id);
            if (trackedTicket is null)
            {
                throw new InvalidOperationException($"Service ticket {ticket.Id} could not be found.");
            }

            trackedTicket.Price += laborCost - trackedTicket.LaborCost;
            trackedTicket.LaborCost = laborCost;
            await context.SaveChangesAsync();
        }

        public async Task UpdateAssignmentAsync(ServiceTicket ticket, int? userId, string changeLogEntry)
        {
            var trackedTicket = await DbSet.SingleOrDefaultAsync(st => st.Id == ticket.Id);
            if (trackedTicket is null)
            {
                throw new InvalidOperationException($"Service ticket {ticket.Id} could not be found.");
            }

            trackedTicket.UserId = userId;
            trackedTicket.ChangeLog.Add(changeLogEntry);
            if (!ReferenceEquals(ticket, trackedTicket))
            {
                ticket.ChangeLog.Add(changeLogEntry);
            }

            await context.SaveChangesAsync();
        }

        public async Task AddEmailSentLogAsync(ServiceTicket ticket, string changeLogEntry)
        {
            var trackedTicket = await DbSet.SingleOrDefaultAsync(st => st.Id == ticket.Id);
            if (trackedTicket is null)
            {
                throw new InvalidOperationException($"Service ticket {ticket.Id} could not be found.");
            }

            trackedTicket.ChangeLog.Add(changeLogEntry);
            if (!ReferenceEquals(ticket, trackedTicket))
            {
                ticket.ChangeLog.Add(changeLogEntry);
            }

            await context.SaveChangesAsync();
        }

        public async Task<decimal> RecalculatePriceAsync(int serviceTicketId)
        {
            var totals = await DbSet
                .Where(serviceTicket => serviceTicket.Id == serviceTicketId)
                .Select(serviceTicket => new
                {
                    serviceTicket.LaborCost,
                    PartsCost = serviceTicket.TicketParts.Sum(ticketPart =>
                        ticketPart.UnitPrice * ticketPart.Quantity)
                })
                .SingleOrDefaultAsync();

            if (totals is null)
            {
                throw new InvalidOperationException($"Service ticket {serviceTicketId} could not be found.");
            }

            var calculatedPrice = totals.LaborCost + totals.PartsCost;
            var trackedTicket = await DbSet.SingleAsync(ticket => ticket.Id == serviceTicketId);

            if (trackedTicket.Price != calculatedPrice)
            {
                trackedTicket.Price = calculatedPrice;
                await context.SaveChangesAsync();
            }

            return calculatedPrice;
        }
    }
}
