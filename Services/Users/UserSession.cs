using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceManagerApp.Services.Users
{
    internal class UserSession
    {
        public SignedInUser? CurrentUser { get; private set; }
        public bool IsSignedIn => CurrentUser is not null;

        public void SignIn(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            CurrentUser = new SignedInUser(
                user.Id,
                user.Name,
                user.Role);

        }

        public void SignOut()
        {
            CurrentUser = null;
        }
    }
}
