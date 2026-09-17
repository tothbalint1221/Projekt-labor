using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class PartRepository(AppDbContext context) : BaseRepository<Part>(context)
    {
    }
}
