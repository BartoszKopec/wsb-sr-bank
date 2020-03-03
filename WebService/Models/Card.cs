using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class Card
    {
        //nr karty 16, data ważności (MM/RR), kod bezpieczeństwa CVV/CCV (3 cyfry)
        private readonly string NumberOfCard;
        private readonly short SaveCode;
        private readonly byte Month;
        private readonly byte Year;

        public Card(string numberOfCard, short saveCode, byte month, byte year)
        {
            NumberOfCard = numberOfCard;
            SaveCode = saveCode;
            Month = month;
            Year = year;
        }

        public string GetNumberOfCard()
        {
            return NumberOfCard;
        }

        public string GetExpirationDate() => string.Format("{0}/{1}", Month, Year); //$"{Month}/{Year}"; //Month.ToString() + "/" + Year.ToString()

        public short GetSaveCode() => SaveCode;
    }
}
