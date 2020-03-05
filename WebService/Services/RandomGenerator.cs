using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Services
{
    public class RandomGenerator
    {
        public static string GetAccountNumber()
        {
            Random random = new Random();
            string accountNumber = string.Empty;
            int accountNumberLenght = 6;
            for (int i = 0; i < accountNumberLenght; i++) //duże znaki od [65; 90]
            {
                char nextChar = (char)random.Next(65, 90);
                accountNumber += nextChar;
            }
            return accountNumber;
        }

    }
}
