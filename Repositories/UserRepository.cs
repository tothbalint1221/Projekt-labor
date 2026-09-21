using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Repositories
{
    public class UserRepository(AppDbContext context) : BaseRepository<User>(context)
    {
    }
}
