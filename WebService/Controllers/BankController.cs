﻿using Core.Models;
using Microsoft.AspNetCore.Mvc;
using WebService.Services;

namespace WebService.Controllers
{
    [Route("bank")]
    public class BankController : ControllerBase
    {
        private readonly BankDb _db;

        public BankController(BankDb db)
        {
            _db = db;
        }

        [HttpPost("transfer")] //http://localhost:5000/bank/transfer?amount=25.00&accountNumberSender=1234&accountNumberReciver=qwer
        public IActionResult TransferMoney([FromBody]TransferData data)
        {
            Payment sender = _db.ReadPayment(data.Sender),
                receiver = _db.ReadPayment(data.Receiver);

            if (sender is null || receiver is null)
                return BadRequest("Błędne numery kont");

            if (data.Amount > sender.AccountBalance)
                return BadRequest("Zbyt mała kwota na koncie");

            sender.AccountBalance -= data.Amount;
            receiver.AccountBalance += data.Amount;

            _db.UpdatePayment(sender);
            _db.UpdatePayment(receiver);

            return Ok(new { Sender = sender, Reciver = receiver });
        }

        [HttpGet("payment/{accountNumber}")] //http://localhost:5000/bank/payment/1234
        public IActionResult GetPayment(string accountNumber)
        {
            Payment payment = _db.ReadPayment(accountNumber);
            if (payment is null)
                return BadRequest("Brak takiego konta płatnościowego");
            else
                return Ok(payment);
        }
    }
}
