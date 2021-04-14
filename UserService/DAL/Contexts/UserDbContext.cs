using UserService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace UserService.DAL
{
    public class UserDbContext : IdentityDbContext<Account>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        //public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
