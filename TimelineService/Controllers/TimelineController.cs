using KwetterShared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimelineService.DAL.Contexts;
using TimelineService.Messaging;

namespace TimelineService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/timeline
    public class TimelineController : Controller
    {
        private readonly TimelineDBContext _context;

        public TimelineController(TimelineDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("read")]
        public async Task<IActionResult> ReadAllKweets()
        {
            try
            {                
                //long oldDate = DateTimeOffset.Now.AddDays(-1).ToUnixTimeSeconds();
                //IQueryable<Kweet> dates = _context.Kweets.Where(kweet => kweet.TimeCreated < oldDate);
                //_context.Kweets.RemoveRange(dates);
                //_context.SaveChanges();

                List<Kweet> kweets = _context.Kweets.ToList();
                return Ok(kweets);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong reading the timeline");
            }
        }
    }
}
