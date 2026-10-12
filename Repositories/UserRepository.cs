using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ServiceManagerApp.Repositories
{
    public class UserRepository(AppDbContext context) : BaseRepository<User>(context)
    {
        public override async Task CreateAsync(User user, bool save = true)
        {
            user.Password = Hash(user.Password);
            await base.CreateAsync(user, save);
        }

        public override async Task UpdateAsync(User user, bool save = true)
        {
            user.Password = Hash(user.Password);
            await base.UpdateAsync(user, save);
        }

        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
        }
        public async Task<(User? User, string? Reason)> LoginAsync(string email, string password)
        {
            var user = await DbSet.SingleOrDefaultAsync(
                u => u.Email == email.Trim());

            if (user == null)
                return (null,"No user was found with this email!");

            bool validPassword = BCrypt.Net.BCrypt.EnhancedVerify(password,
                user.Password);

            if (!validPassword)
                return (null, "Incorrect password.");

            return (user,null);
        }
    }

}
