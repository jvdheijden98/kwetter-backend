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

            if (context.Users.Any())
            {
                return;
            }

            // Can be done with lists, arrays have better performance
            Account[] accounts = new Account[]
            {
                new Account { UserName="ANiceAccount", Email="Anice@gmail.com", PasswordHash="AHashedPassword1" },
                new Account { UserName="BlueBella", Email="BlueBella@gmail.com", PasswordHash="BrokenHashedPassword2" },
                new Account { UserName="CC3", Email="CamillaCameriere@gmail.com", PasswordHash="ControllingHashedpassword3" }
            };

            foreach(Account account in accounts)
            {
                context.Users.Add(account);
            }
            context.SaveChanges();
        }
    }
}
