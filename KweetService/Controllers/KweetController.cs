using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KweetService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/kweet
    public class KweetController : Controller
    {
        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateKweet()
        {

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
