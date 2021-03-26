using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.DAL;
using UserService.Models;

namespace UserService.Logic
{
    public class AuthenticationLogic
    {
        private readonly UserDbContext _context;

        public AuthenticationLogic(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> Index()
        {
            List<Account> accounts = await _context.Accounts.ToListAsync();
            return accounts;
        }

        public async Task<Account> Details(int id)
        {
            Account account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountID == id);
            return account;
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task Create(Account account)
        {
            _context.Add(account);            
            await _context.SaveChangesAsync();
        }

        /*public async Task<IActionResult> Edit(int id, [Bind("AccountID,Username,Password")] Account account)
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

        public async Task Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }

        public bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountID == id);
        }
    }
}
