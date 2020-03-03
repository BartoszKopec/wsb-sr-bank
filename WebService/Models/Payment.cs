using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; }

        public decimal AccountBalance { get; set; }
    }
}
