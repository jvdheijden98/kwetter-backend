using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(UserDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Accounts.Any())
            {
                return;
            }

            // Kan met Lists, arrays is beter performance.
            Account[] accounts = new Account[]
            {
                new Account { Username="ANiceAccount", Password="APassword1", Role="Administrator" },
                new Account { Username="BlueBella", Password="BrokenPassword2", Role="Moderator" },
                new Account { Username="CC3", Password="Controllingpassword3", Role="User" }
            };

            foreach(Account account in accounts)
            {
                context.Accounts.Add(account);
            }
            context.SaveChanges();
        }
    }
}
