using KweetService.DAL.Contexts;
using KweetService.Messaging;
using KweetService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KwetterShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace KweetService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/kweet
    public class KweetController : Controller
    {
        private readonly KweetDBContext _context;
        private readonly HttpClient _httpClient;

        public KweetController(KweetDBContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task<string> CensorCurses(string kweetMessage)
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(kweetMessage);
            StringContent content = new StringContent(kweetMessage, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("https://kwetterfunctions.azurewebsites.net/api/censorcurses", content);
            string censoredKweetMessage = httpResponseMessage.Content.ReadAsStringAsync().Result;

            return censoredKweetMessage;
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

            //Regex regex = new Regex(@"^\p{L}+$"); // TODO Allow spaces
            Regex regex = new Regex(@"^[\p{L}\s\w?!.,@#$%^&*()_+-]+$");
            if (!regex.IsMatch(kweetRequest.Message))
            {
                return BadRequest("Please provide input that is UTF-8 valid.");
            }

            string censoredKweetMessage = await CensorCurses(kweetRequest.Message);

            // Maybe store the ID in claims in the future instead... 
            // Will require a database call upon registering since database creates them.
            string username = User.Identity.Name;
            Kweet kweet = new Kweet
            {
                Username = username,
                Message = censoredKweetMessage,
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

            try
            {
                RabbitPublisher.PublishMessage(kweet);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status201Created, "Rabbit FAILED to publish message"); // Good behaviour would be to cache the message and try again later. User gets what they want, but bhind screens in progress.
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        // Test method - Delete when timeline service is setup (and then make a longterm one here)
        [HttpGet]
        [Route("read")]
        public async Task<IActionResult> ReadAllKweets()
        {
            try
            {
                List<Kweet> kweets = _context.Kweets.ToList();
                return Ok(kweets);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "oopsiewoopsie");
            }
        }

        // TODO: Update & Delete must only be able to delete own tweets. (unless admin)
        [Authorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateKweet([FromBody] KweetChangeRequest kweetChangeRequest)
        {
            try
            {
                Kweet kweetToChange = _context.Kweets.Find(kweetChangeRequest.KweetID);
                kweetToChange.Message = kweetChangeRequest.Message;
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteKweet([FromBody] KweetDeleteRequest kweetDeleteRequest)
        {
            try
            {
                Kweet kweetToDelete = _context.Kweets.Find(kweetDeleteRequest.KweetID);
                _context.Kweets.Remove(kweetToDelete);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            return Ok();
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
