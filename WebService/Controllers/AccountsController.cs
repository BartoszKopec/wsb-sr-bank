using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Models;
using WebService.Services;

namespace WebService.Controllers
{
    [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly BankDb _db;

        public AccountsController(BankDb db)
        {
            _db = db;
        }

        //crud - create read update delete

        [HttpPost("insert")]
        public IActionResult Insert([FromBody]Account account)
        {
            int id = _db.AddAccount(account);
            return Created($"/accounts/get/{id}", new { Id = id });
        }

        [HttpGet("get/{id}")]
        public IActionResult GetOne(int id)
        {
            Account account = _db.ReadAccount(id);
            
            if(account is null)
            {
                return BadRequest("Nie ma takiego konta");
            }
            else
            {
                return Ok(account);
            }
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            List<Account> accounts = _db.ReadAccounts();

            if (accounts.Count == 0)
                return BadRequest("Nie ma listy kont");
            else
                return Ok(accounts);
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody]Account account)
        {
            bool status = _db.UpdateAccount(account);

            if (status is true)
                return Ok();
            else
                return BadRequest("Nie ma takiego konta");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            bool status = _db.DeleteAccount(id);

            if (status is true)
                return Ok();
            else
                return BadRequest("Nie ma takiego konta");
        }
    }
}
