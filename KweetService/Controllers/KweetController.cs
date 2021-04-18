using KweetService.DAL.Contexts;
using KweetService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KweetService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/kweet
    public class KweetController : Controller
    {
        private readonly KweetDBContext _context;

        public KweetController(KweetDBContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateKweet([FromBody] KweetRequest kweetRequest)
        {
            if(kweetRequest.Message.Length > 144)
            {
                return BadRequest("Message can't be longer that 144 characters");
            }

            Regex regex = new Regex(@"^\p{L}+$"); // TODO Allow spaces
            if (!regex.IsMatch(kweetRequest.Message))
            {
                return BadRequest("Please provide input that is UTF-8 valid.");
            }

            // Maybe store the ID in claims in the future instead... 
            // Will require a database call upon registering since database creates them.
            string username = User.Identity.Name;
            Kweet kweet = new Kweet
            {
                Username = username,
                Message = kweetRequest.Message,
                Likes = 0,
                TimeCreated = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            try
            {
                _context.Add(kweet);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }            

            return StatusCode(StatusCodes.Status201Created);
        }

        // Works
        [Authorize]
        [HttpGet]
        [Route("test")]
        public IActionResult CheckTokenInformation()
        {
            return Ok(User.Identity.Name);
        }
    }
}
