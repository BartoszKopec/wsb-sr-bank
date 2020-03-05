using System.Collections.Generic;

namespace Core.Models
{
    public class Account
    {
        //imię, nazwisko, PESEL, Stan środków na koncie, nr konta, kolekcja kart
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
