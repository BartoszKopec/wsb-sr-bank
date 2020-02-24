using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class Account
    {
        //imię, nazwisko, PESEL, Stan środków na koncie, nr konta, kolekcja kart

        private readonly string firstName;
        private readonly string lastName;
        private readonly string pesel;
        private decimal accountBalance;
        private readonly string accountNumber;
        private List<Card> cards;

        public Account(string firstName, string lastName, string pesel, decimal accountBalance, string accountNumber, List<Card> cards)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.pesel = pesel;
            this.accountBalance = accountBalance;
            this.accountNumber = accountNumber;
            this.cards = cards;
        }

        public string FirstName => firstName;
        public string LastName => lastName;
        public string Pesel => pesel;

        public string AccountNumber => accountNumber;

        //public decimal AccountBalance 
        //{ 
        //    get => accountBalance; 
        //    set => accountBalance = value; 
        //}
        public decimal AccountBalance
        {
            get
            {
                return accountBalance;
            }
            set
            {
                accountBalance = value;
            }
        }


        public List<Card> Cards => cards;
    }
}
