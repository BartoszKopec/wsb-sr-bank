using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class TransferData
    {
        public decimal Amount { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}
