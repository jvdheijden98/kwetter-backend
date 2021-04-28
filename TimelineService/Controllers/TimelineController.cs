using KwetterShared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimelineService.Messaging;

namespace TimelineService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/timeline
    public class TimelineController : Controller
    {
        [HttpGet]
        [Route("readlast")]
        public async Task<IActionResult> ReadLastKweet()
        {
            Kweet lastKweet = RabbitSubscriber.LastKweet;
            if (lastKweet != null)
            {
                return Ok(lastKweet);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "woopsieoopsie");
        }

        [HttpGet]
        [Route("read")]
        public async Task<IActionResult> ReadAllKweets()
        {
            try
            {
                List<Kweet> kweets = new List<Kweet>();
                return Ok(kweets);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "oopsiewoopsie");
            }
        }
    }
}
