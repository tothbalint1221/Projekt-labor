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
        
        public async Task<(bool Success,string? Error)> ChangePasswordAsync(int userId,string currentPassword, string newPassword)
        {
            if(string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                return (false, "Please fill in all password fields");
            }

            var user = await DbSet
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.Id == userId);
            
            if(user is null)
            {
                return (false, "User not found");
            }

            bool validPassword = BCrypt.Net.BCrypt.EnhancedVerify(
                currentPassword,
                user.Password);

            if(!validPassword)
            {
                return (false, "Your current password is incorrect.");
            }

            if(currentPassword==newPassword)
            {
                return (false, "Please choose a different new password");
            }

            string newHash = Hash(newPassword);

            int updatedRows = await DbSet
                .Where(u => u.Id == userId && u.Password == user.Password)
                .ExecuteUpdateAsync(update => update
                        .SetProperty(u => u.Password, newHash));

            if(updatedRows != 1)
            {
                return (false,
                    "The user data changed. Please try again.");
            }

            return (true, null);

        }

    }
}
