using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ServiceManagerApp.Repositories
{
    public class FaultRepository(AppDbContext context) : BaseRepository<Fault>(context)
    {
        public async Task<List<Fault>> GetByServiceTicketIdAsync(int serviceTicketId)
        {
            return await DbSet
                .Where(fault => fault.ServiceTicketId == serviceTicketId)
                .ToListAsync();
        }

        public async Task<List<Fault>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await DbSet
                .Include(fault => fault.ServiceTicket)
                .Where(fault => fault.ServiceTicket.EquipmentId == equipmentId)
                .ToListAsync();
        }
    }
}
