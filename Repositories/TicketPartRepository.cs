using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ServiceManagerApp.Repositories
{
    public class TicketPartRepository(AppDbContext context) : BaseRepository<TicketPart>(context)
    {
        public async Task<List<TicketPart>> GetByServiceTicketIdAsync(int serviceTicketId)
        {
            var ticketParts = await DbSet
                .Include(tp => tp.Part)
                .Where(tp => tp.ServiceTicketId == serviceTicketId)
                .ToListAsync();

            var repairedLegacyPrices = false;
            foreach (var ticketPart in ticketParts)
            {
                if (ticketPart.UnitPrice == 0 && ticketPart.Part.PartCost > 0)
                {
                    ticketPart.UnitPrice = ticketPart.Part.PartCost;
                    repairedLegacyPrices = true;
                }
            }

            if (repairedLegacyPrices)
            {
                await context.SaveChangesAsync();
            }

            return ticketParts;
        }

        public async Task ChangeQuantityAsync(int ticketPartId, bool increase)
        {
            var ticketPart = await DbSet.SingleOrDefaultAsync(tp => tp.Id == ticketPartId)
                ?? throw new InvalidOperationException("Ticket part could not be found.");

            var part = await context.Parts.SingleOrDefaultAsync(p => p.Id == ticketPart.PartId)
                ?? throw new InvalidOperationException("Part could not be found.");

            var ticket = await context.ServiceTickets.SingleOrDefaultAsync(t => t.Id == ticketPart.ServiceTicketId)
                ?? throw new InvalidOperationException("Service ticket could not be found.");

            if (increase)
            {
                if (part.Quantity < 1)
                    throw new InvalidOperationException("There is no stock available.");

                ticketPart.Quantity++;
                part.Quantity--;
            }
            else
            {
                if (ticketPart.Quantity <= 1)
                    return;

                ticketPart.Quantity--;
                part.Quantity++;
            }

            await RecalculatePriceAsync(ticket);
            await context.SaveChangesAsync();
        }

        public override async Task CreateAsync(TicketPart tp, bool save = true)
        {
            var part = await context.Parts.SingleOrDefaultAsync(p => p.Id == tp.PartId)
                ?? throw new InvalidOperationException("Part could not be found.");

            var ticket = await context.ServiceTickets.SingleOrDefaultAsync(t => t.Id == tp.ServiceTicketId)
                ?? throw new InvalidOperationException("Service ticket could not be found.");

            if (tp.Quantity < 1)
            {
                throw new InvalidOperationException("Ticket part quantity must be greater than zero.");
            }

            if (part.Quantity < tp.Quantity)
            {
                throw new InvalidOperationException("There is not enough stock available.");
            }

            tp.UnitPrice = part.PartCost;
            part.Quantity -= tp.Quantity;
            await context.AddAsync(tp);
            await RecalculatePriceAsync(ticket);
            if (save) await context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(TicketPart tp, bool save = true)
        {
            var ticketPart = await DbSet.SingleOrDefaultAsync(item => item.Id == tp.Id)
                ?? throw new InvalidOperationException("Ticket part could not be found.");

            var part = await context.Parts.SingleOrDefaultAsync(p => p.Id == ticketPart.PartId)
                ?? throw new InvalidOperationException("Part could not be found.");

            var ticket = await context.ServiceTickets.SingleOrDefaultAsync(t => t.Id == ticketPart.ServiceTicketId)
                ?? throw new InvalidOperationException("Service ticket could not be found.");


            part.Quantity += ticketPart.Quantity;

            context.Remove(ticketPart);
            await RecalculatePriceAsync(ticket);
            if (save) await context.SaveChangesAsync();
        }

        private async Task RecalculatePriceAsync(ServiceTicket ticket)
        {
            var persistedParts = await DbSet
                .Where(ticketPart => ticketPart.ServiceTicketId == ticket.Id)
                .ToListAsync();
            var deletedIds = context.ChangeTracker.Entries<TicketPart>()
                .Where(entry => entry.State == EntityState.Deleted)
                .Select(entry => entry.Entity.Id)
                .ToHashSet();
            var addedParts = context.ChangeTracker.Entries<TicketPart>()
                .Where(entry => entry.State == EntityState.Added &&
                                entry.Entity.ServiceTicketId == ticket.Id)
                .Select(entry => entry.Entity);

            ticket.Price = ticket.LaborCost +
                persistedParts
                    .Where(ticketPart => !deletedIds.Contains(ticketPart.Id))
                    .Sum(ticketPart => ticketPart.UnitPrice * ticketPart.Quantity) +
                addedParts.Sum(ticketPart => ticketPart.UnitPrice * ticketPart.Quantity);
        }
    }
}
