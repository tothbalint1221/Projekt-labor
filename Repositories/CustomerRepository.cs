using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class CustomerRepository(AppDbContext context) : BaseRepository<Customer>(context)
    {

    }
}
