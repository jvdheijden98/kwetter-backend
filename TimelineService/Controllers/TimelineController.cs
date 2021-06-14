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

                List<Kweet> kweets = _context.Kweets.ToList();
                kweets.Reverse();
                return Ok(kweets);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong reading the timeline");
            }
        }
    }
}
