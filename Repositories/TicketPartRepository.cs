using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class TicketPartRepository(AppDbContext context) : BaseRepository<TicketPart>(context)
    {
    }
}
