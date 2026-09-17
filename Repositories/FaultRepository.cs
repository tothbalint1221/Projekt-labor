using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class FaultRepository(AppDbContext context) : BaseRepository<Fault>(context)
    {
    }
}
