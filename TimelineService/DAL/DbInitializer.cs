using KwetterShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimelineService.DAL.Contexts;

namespace TimelineService.DAL
{
    public class DbInitializer
    {
        public static void Initialize(TimelineDBContext context)
        {
            context.Database.EnsureCreated();

            if (context.Kweets.Any())
            {
                return;
            }

            // Can be done with lists, arrays have better performance
            Kweet[] kweets = new Kweet[]
            {
                new Kweet { Username="ANiceAccount", Likes=0, Message="beepboop kweet", TimeCreated=1618522342 }
            };

            foreach (Kweet kweet in kweets)
            {
                context.Kweets.Add(kweet);
            }
            context.SaveChanges();
        }
    }
}
