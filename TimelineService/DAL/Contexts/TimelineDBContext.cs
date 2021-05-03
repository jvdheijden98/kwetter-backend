using KwetterShared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimelineService.DAL.Contexts
{
    public class TimelineDBContext : DbContext
    {
        public TimelineDBContext(DbContextOptions<TimelineDBContext> options) : base(options)
        {
        }

        public DbSet<Kweet> Kweets { get; set; }
    }
}
