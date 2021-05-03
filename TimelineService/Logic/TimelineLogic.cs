using KwetterShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimelineService.DAL.Contexts;

namespace TimelineService.Logic
{
    public class TimelineLogic
    {
        private readonly TimelineDBContext _context;

        public TimelineLogic(TimelineDBContext context)
        {
            _context = context;
        }

        public async void CreateKweet(Kweet kweet)
        {
            try
            {
                _context.Add(kweet);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
