using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Logic;
using UserService.Models;

namespace UserService.Controllers
{    
    [ApiController]
    [Route("api/[controller]")] // api/authentication
    public class AuthenticationController : Controller
    {
        private readonly AuthenticationLogic _authenticationLogic;

        public AuthenticationController(AuthenticationLogic authenticationLogic)
        {
            _authenticationLogic = authenticationLogic;
        }

        // GET: api/authentication/index
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Account> accounts = await _authenticationLogic.Index();
            return Ok(accounts);
        }

        // GET: api/authentication/details/
        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _authenticationLogic.Details((int)id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // POST: api/authentication/register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("AccountID, Username, Password")] Account account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _authenticationLogic.Create(account);
                }
                catch (DbUpdateException e)
                {
                    return StatusCode(500, e.Message);  // Internal Server Error
                }
            }
            return StatusCode(201); // Created
        }

        // PATCH: api/authentication/edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountID,Username,Password")] Account account)
        {
            if (id != account.AccountID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }*/

        // DELETE: api/authentication/delete/5
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authenticationLogic.AccountExists(id))
            {
                return BadRequest();
            }

            try
            {
                await _authenticationLogic.Delete(id);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, e.Message);  // Internal Server Error
            }

            return NoContent();
        }
    }
}
