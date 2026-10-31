using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Services
{
    public sealed record SignedInUser(
        int Id, string Name, Role Role);

    internal class UserSession
    {

    }
}
