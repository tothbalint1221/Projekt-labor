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
            return await DbSet.Include(st => st.User).Include(st => st.Equipment).ToListAsync();
        }
    }
}
