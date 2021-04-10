using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UserService.DAL;
using UserService.JWT;
using UserService.Models;


namespace UserService.Logic
{
    public class AuthenticationLogic
    {
        private readonly UserDbContext _context;
        private readonly JwtAuthManager _jwtAuthManager;
        private readonly UserManager<Account> _userManager;

        public AuthenticationLogic(UserDbContext context, JwtAuthManager jwtAuthManager, UserManager<Account> userManager)
        {
            _context = context;
            _jwtAuthManager = jwtAuthManager;
            _userManager = userManager;
        }

        #region Basic CRUD
        public async Task<IEnumerable<Account>> Index()
        {
            List<Account> accounts = await _context.Accounts.ToListAsync();
            return accounts;
        }

        /*public async Task<Account> Details(int id)
        {
            Account account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountID == id);
            return account;
        }
        */

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task Create(RegisterRequest registerRequest)   // TODO: Password Hashing
        {
            if (AccountExists(registerRequest.Username).Result)
            {
                throw new Exception("Username already in use");
            }

            Account account = new Account { Username = registerRequest.Username };
            account.SecurityStamp = Guid.NewGuid().ToString(); // Should when a password change occurs, should invalidates old JWT tokens so old one cant be used anymore

            IdentityResult result = await _userManager.CreateAsync(account, registerRequest.Password); // Including password here to use the hashfunction of the usermanager

            if (result.Succeeded)
            {
                // TODO: Logging, account creation with password
                // Optionally add Email confirmation here

                //_context.Add(account);
                //await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Account creation failed.");
            }
        }

        public async Task Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }

        #endregion

        // TODO: Password Hashing
        public async Task<LoginResponse> Login(string username, string password)
        {
            if (!AccountExists(username).Result)
            {
                throw new Exception("No such user found");
            }

            if (!PasswordMatches(username, password).Result)
            {
                throw new Exception("Password doesn't match.");
            }

            string role = await Task.Run(() => _context.Accounts.Where(e => e.Username == username).Single().Role);

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(username, claims, DateTime.Now);
            // TODO: Logging, user logged in.

            LoginResponse loginResponse = new LoginResponse
            {
                UserName = username,
                Role = role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
            return loginResponse;
        }

        public async void Logout(string username)
        {
            _jwtAuthManager.RemoveRefreshTokenByUserName(username);
            // TODO Logging, user logged out
        }

        public async Task<LoginResponse> RefreshToken(string accessToken, string refreshToken, string username, string role)
        {
            // TODO: Logging, User trying to refresh JWT token.
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new Exception("No valid Refreshtoken supplied");
            }

            var jwtResult = _jwtAuthManager.Refresh(refreshToken, accessToken, DateTime.Now);
            // TODO: Logging, User has refreshed JWT token.

            LoginResponse loginResponse = new LoginResponse
            {
                UserName = username,
                Role = role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
            return loginResponse;
        }

        public bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountID == id);
        }

        private async Task<bool> AccountExists(string username)
        {
            bool accountExists = await Task.Run(() => _context.Accounts.Any(e => e.Username == username));
            return accountExists;
        }

        private async Task<bool> PasswordMatches(string username, string password)
        {
            bool passwordMatches = await Task.Run(() => _context.Accounts.Any(e => e.Username == username && e.Password == password));
            return passwordMatches;
        }
    }
}
