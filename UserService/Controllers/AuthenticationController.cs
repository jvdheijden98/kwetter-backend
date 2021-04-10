using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.JWT;
using UserService.Logic;
using UserService.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

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
        /*[HttpGet]
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
        */

        // POST: api/authentication/register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("register")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // TODO: Validate Input (no funky input)
            // TODO: Check duplicate username
            // TODO: Add default role
            // TODO Change parameter binding to request body

            try
            {
                await _authenticationLogic.Create(registerRequest);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, e.Message);  // Internal Server Error
            }

            return StatusCode(201); // Created
        }

        // POST: api/authentication/login
        [HttpPost]
        [Route("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            LoginResponse loginResponse;
            try
            {
                loginResponse = await _authenticationLogic.Login(loginRequest.Username, loginRequest.Password);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            return Ok(loginResponse);
        }

        // 
        [HttpGet("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            string username = User.Identity?.Name;
            _authenticationLogic.Logout(username);

            return Ok();
        }

        // (TODO: Clear refreshtoken cache every so often)
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshRequest)
        {
            string username = User.Identity.Name;
            string role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            string accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            LoginResponse loginResponse;
            try
            {
                loginResponse = await _authenticationLogic.RefreshToken(accessToken, refreshRequest.RefreshToken, username, role);
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return Unauthorized();
            }

            return Ok(loginResponse);
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
