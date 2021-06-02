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
using System.Web.Helpers;

namespace KweetService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/kweet
    public class KweetController : Controller
    {
        private readonly KweetDBContext _context;
        
        // TODO Implement Responses
        public KweetController(KweetDBContext context)
        {
            _context = context;
        }

        public async Task<string> CensorCurses(string kweetMessage)
        {            
            HttpClient _httpClient = new HttpClient(); ;

            // New Request Message
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://kwetterfunctions.azurewebsites.net/api/censorcurses");
            //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:7071/api/CensorCurses");        

            // Add body
            requestMessage.Content = new StringContent(kweetMessage, Encoding.UTF8, "application/json");

            // Send request
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

            string censoredKweetMessage = await responseMessage.Content.ReadAsStringAsync();
            return censoredKweetMessage;
        }


        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateKweet([FromBody] KweetRequest kweetRequest)
        {
            Response response;
            if (kweetRequest.Message.Length > 144)
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
            //Console.WriteLine("Post method: " + censoredKweetMessage);

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

            response = new Response { Status = "Succes", Message = "Kweet created succesfully." };
            return StatusCode(StatusCodes.Status201Created, response);
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
