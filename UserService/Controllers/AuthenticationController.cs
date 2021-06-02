using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using UserService.Authentication;
using KwetterShared.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/authentication
    public class AuthenticationController : Controller
    {
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        // Constructor
        public AuthenticationController(UserManager<Account> userManager, RoleManager<IdentityRole> roleManager,
                                        IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            Response response;
            Account account = await _userManager.FindByNameAsync(registerRequest.Username);
            if (account != null)
            {
                response = new Response { Status = "Error", Message = "User already exists!" };
                return StatusCode(StatusCodes.Status409Conflict, response);
            }

            account = new Account()
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(), // Should when a password change occurs, should invalidates old JWT tokens so old one cant be used anymore                
            };

            // CreateAsync uses EntityFramework and the context to save the account into DB
            // Overloading with the password, hashes the password
            IdentityResult result = await _userManager.CreateAsync(account, registerRequest.Password);
            if (!result.Succeeded)
            {
                response = new Response { Status = "Error", Message = "User creation failed! Check your details and try again." };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            response = new Response { Status = "Succes", Message = "User created succesfully." };
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest registerRequest)
        {
            Response response;
            Account account = await _userManager.FindByNameAsync(registerRequest.Username);
            if(account != null)
            {
                response = new Response { Status = "Error", Message = "User already exists!" };
                return StatusCode(StatusCodes.Status409Conflict, response);
            }

            account = new Account()
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),             
            };

            IdentityResult result = await _userManager.CreateAsync(account, registerRequest.Password);
            if (!result.Succeeded)
            {
                response = new Response { Status = "Error", Message = "User creation failed! Check your details and try again." };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            if(!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Moderator))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Moderator));
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }                

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(account, UserRoles.Admin);
            }

            response = new Response { Status = "Succes", Message = "User created succesfully." };
            return StatusCode(StatusCodes.Status201Created, response);
        }

        // POST: /api/authentication/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            Account account = await _userManager.FindByNameAsync(loginRequest.Username);
            if (account == null || !await _userManager.CheckPasswordAsync(account, loginRequest.Password))
            {
                return Unauthorized();
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(account);
            List<Claim> authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (string userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            SymmetricSecurityKey authSigningkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningkey, SecurityAlgorithms.HmacSha256)
                );

            var tokenResponse = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo
            };
            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpGet]
        [Route("authorizetest")]
        public IActionResult AuthorizeTest()
        {
            return Ok("Hello World");
        }
    }
}
