using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class TransferResult
    {
        public Payment Sender { get; set; }
        public Payment Receiver { get; set; }
    }
}
