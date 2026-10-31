using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Services.Users
{
    public sealed record SignedInUser(
        int Id, string Name, Role Role);

}
