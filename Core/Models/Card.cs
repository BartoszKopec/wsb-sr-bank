namespace Core.Models
{
    public class Card
    {
        //nr karty 16, data ważności (MM/RR), kod bezpieczeństwa CVV/CCV (3 cyfry)

        public string NumberOfCard { get; set; }
        public short SafeCode { get; set; }
        public byte Month { get; set; }
        public byte Year { get; set; }

        public string GetExpirationDate() => string.Format("{0}/{1}", Month, Year);
    }
}
