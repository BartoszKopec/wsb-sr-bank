using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Controllers
{
    [Route("bank")]
    public class BankController : ControllerBase
    {
        private static readonly List<Account> accounts = new List<Account>
        {
            new Account("Jan", "Kowalski", "12345678901", 100M, "NR KONTA", null),
            new Account("Jan", "Kowalski", "12345678901", 100M, "NR KONTA", null),
            new Account("Jan", "Kowalski", "12345678901", 100M, "NR KONTA", null),
        };

        [HttpPost("transfer")]
        public IActionResult TransferMoney(decimal amount, string accountNumberSender, string accountNumberReciver)
        {
            Account accountSender=null, accountReceiver=null;

            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                if (account.AccountNumber == accountNumberSender)
                    accountSender = account;
                if (account.AccountNumber == accountNumberReciver)
                    accountReceiver = account;
            }

            if (accountSender == null || accountReceiver == null)
            {
                return BadRequest("błędne numery kont");
            }

            decimal newSenderAmount = Transfer(accountSender, accountReceiver, amount);
            return Ok(newSenderAmount);
        }


        private decimal ChangeAccountBalance(Account account, decimal amount)
        {
            account.AccountBalance += amount;
            return account.AccountBalance;
        }

        private decimal Transfer(Account accountSender, Account accountReceiver, decimal amount)
        {
            accountSender.AccountBalance -= amount;
            accountReceiver.AccountBalance += amount;

            return accountSender.AccountBalance;
        }

    }
}
