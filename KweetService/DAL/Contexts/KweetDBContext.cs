using KweetService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KweetService.DAL.Contexts
{
    public class KweetDBContext : DbContext
    {
        public KweetDBContext(DbContextOptions<KweetDBContext> options) : base(options)
        {
        }

        public DbSet<Kweet> Kweets { get; set; }
    }
}
