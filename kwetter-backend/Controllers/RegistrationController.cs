using kwetter_backend.Logic;
using kwetter_backend.TempLocalPersistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace kwetter_backend.Controllers
{
    [Route("api/[controller]")] // api/registration
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly AccountLogic _accountLogic;

        public RegistrationController(AccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        // GET: api/registration
        [HttpGet]
        [Route("accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            List<Account> accounts = _accountLogic.GetAccounts();
            return Ok(accounts);
        }

        // GET: api/registration/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            Account account = _accountLogic.GetAccount(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        /*
        // PUT: api/registration/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            if (!AccountExists(id))
            {
                return NotFound();
            }

            try
            {
                await _accountLogic.PutAccount(account);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return NoContent();
        }
        */

        // POST: api/registration
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            try
            {
                _accountLogic.PostAccount(account);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: api/registration/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (!AccountExists(id))
            {
                return NotFound();
            }
            _accountLogic.DeleteAccount(id);

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            bool accountExists = _accountLogic.AccountExists(id);
            return accountExists;
        }
    }
}
