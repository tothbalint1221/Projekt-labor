using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class ServiceTicketRepository(AppDbContext context) : BaseRepository<ServiceTicket>(context)
    {
    }
}
