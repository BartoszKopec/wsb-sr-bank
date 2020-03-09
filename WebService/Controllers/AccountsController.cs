using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [HttpPost("insert")]
        public IActionResult Insert([FromBody]Account account)
        {
            if (_db.ReadAccounts().Find(a => a.Pesel == account.Pesel) is null)
            {
                int id = _db.AddAccount(account);
                account.Id = id;
                return Created($"/accounts/get/{id}", account);
            }
            else
            {
                return BadRequest("Taka osoba ma już konto.");
            }
        }

        [HttpGet("get/{id}")]
        public IActionResult GetOne(int id)
        {
            Account account = _db.ReadAccount(id);

            if (account is null)
            {
                return BadRequest("Nie ma takiego konta");
            }
            else
            {
                return Ok(account);
            }
        }

        //------------------------------------------------------------------------------------
        [HttpGet("get/{AccountNumber}")]
        public IActionResult GetMy(string AccountNumber)
        {
            Payment number = _db.ReadAccount(AccountNumber);

            if (account is null)
            {
                return BadRequest("Nie ma takiego konta");
            }
            else
            {
                return Ok(account);
            }
        }
        //---------------------------------------------------------------------------------------





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

        [HttpPut("{id}/addcard")]
        public IActionResult AddCardToAccount(int id)
        {
            bool status = _db.AddCard(id);
            if (status is true)
                return Ok();
            else
                return BadRequest("Nie ma takiego konta");
        }

        [HttpDelete("{id}/removecard/{cardId}")]
        public IActionResult DeleteCardFromAccount(int id, string cardId)
        {
            bool status = _db.RemoveCard(id, cardId);
            if (status is true)
                return Ok();
            else
                return BadRequest("Nie ma takiego konta");
        }

    }
}
