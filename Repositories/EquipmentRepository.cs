using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class EquipmentRepository(AppDbContext context) : BaseRepository<Equipment>(context)
    {
    }
}
