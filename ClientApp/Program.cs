using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClientApp.Services;
using System.Globalization;

namespace ClientApp
{
    class Program
    {
        private static CancellationTokenSource _tokenSource;
        private static readonly NumberFormatInfo formatInfo = new CultureInfo("en").NumberFormat;

        static async Task Main()
        {
            _tokenSource = new CancellationTokenSource();
            Console.WriteLine("Aplikacja klienta banku");
            await DrawUI();
        }

        static async Task DrawUI()
        {
            bool isEnd = false;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Podaj komendę (wpłata, wypłata, przelew, konto, kupno, wyjście):");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "wpłata": await AddMoney(); break;
                    case "wypłata": await WithdrawMoney(); break;
                    case "konto": await ShowMyAccount(); break;
                    case "wyjście": isEnd = true; break;
                    case "przelew": await Transfer();break;
                    case "kupno": await BuySomething(); break;

                    default:
                        break;
                }
            } while (!isEnd);
        }

        static async Task ShowMyAccount()
        {
            //klient podaje numer konta
            //pobiera z bazy klasy Payment dane zgodne z numerem konta
            //jeśli się uda pobrać drukuje numer oraz kwotę na koncie

            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Pokazywanie zawartości konta konta.");
            Console.WriteLine("Podaj Numer Twojego konta:");
            string number = Console.ReadLine();
            //numer Id w bazie Account i Payment są identyczne!!
            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);//pociągnięcie danych

            if (isSucces)
            {
                Payment payment = JsonConvert.DeserializeObject<Payment>(content);
                Console.WriteLine();
                Console.WriteLine(string.Format(
                    "Na koncie nr: {0}\n obecnie znajduje się następująca kwota środków w złotych: {1}",
                    payment.AccountNumber, payment.AccountBalance
                    ));
            }
            else
                Console.WriteLine(content);
        }

        static async Task AddMoney()
        {
            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.WriteLine("Wpłacanie do wpłatomatu.");
            Console.WriteLine("Podanie numeru konta:");
            string number = Console.ReadLine();

            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);
            if (isSucces)
            {
                Payment receiver = JsonConvert.DeserializeObject<Payment>(content);
                Console.WriteLine("Proszę przygotować pieniądze do wpłaty.");
                Console.WriteLine("Kwota:");
                decimal amount = decimal.Parse(Console.ReadLine(), formatInfo);

                TransferData data = new TransferData
                {
                    Sender = "NJIMQF", //nr wpłatomatu
                    Receiver = receiver.AccountNumber,
                    Amount = amount
                };

                (isSucces, content) = await api.PostTransfer(data, _tokenSource.Token);
                if (isSucces)
                {
                    TransferResult transferResult = JsonConvert.DeserializeObject<TransferResult>(content);
                    Console.WriteLine("Masz obecnie na koncie " + transferResult.Receiver.AccountBalance + " zł.");
                }
                else
                    Console.WriteLine(content);
            }
            else
                Console.WriteLine(content);
        }

        static async Task WithdrawMoney()
        {
            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Wypłacanie z bankomatu.");
            Console.WriteLine("Podanie numeru:");
            string number = Console.ReadLine();

            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);
            if (isSucces)
            {
                Payment sender = JsonConvert.DeserializeObject<Payment>(content);
                Console.WriteLine("Kwota:");
                decimal amount = decimal.Parse(Console.ReadLine(), formatInfo);

                TransferData data = new TransferData
                {
                    Receiver = "XABQKR",
                    Sender = sender.AccountNumber,
                    Amount = amount
                };

                (isSucces, content) = await api.PostTransfer(data, _tokenSource.Token);
                if (isSucces)
                {
                    TransferResult transferResult = JsonConvert.DeserializeObject<TransferResult>(content);
                    Console.WriteLine("Masz obecnie na koncie " + transferResult.Sender.AccountBalance + " zł.");
                }
                else
                    Console.WriteLine(content);
            }
            else
                Console.WriteLine(content);
        }

        static async Task BuySomething()
        {
            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Kupno.");
            Console.WriteLine("Podanie numeru:");
            string number = Console.ReadLine();

            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);
            if (isSucces)
            {
                Payment sender = JsonConvert.DeserializeObject<Payment>(content);
                Console.WriteLine("Kwota:");
                decimal Amount = decimal.Parse(Console.ReadLine(), formatInfo);

                TransferData data = new TransferData
                {
                    Receiver = "IMNXDE", //konto sklepu
                    Sender = sender.AccountNumber,
                    Amount = Amount
                };

                (isSucces, content) = await api.PostTransfer(data, _tokenSource.Token);
                if (isSucces)
                {
                    TransferResult transferResult = JsonConvert.DeserializeObject<TransferResult>(content);
                    Console.WriteLine("Masz obecnie na koncie " + transferResult.Sender.AccountBalance + " zł.");
                }
                else
                    Console.WriteLine(content);
            }
            else
                Console.WriteLine(content);
        }

        static async Task Transfer()
        {
            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Przelew.");
            Console.WriteLine("Podanie numeru konta przekazującego środki:");
            string numberSender = Console.ReadLine();
         
            (bool isSucces, string content) = await api.GetPaymentData(numberSender, _tokenSource.Token);
            if (isSucces)
            {
                Payment sender = JsonConvert.DeserializeObject<Payment>(content);
                Console.WriteLine();
                Console.WriteLine("Proszę podać numer konta odbiorcy");
                string numberReceiver = Console.ReadLine();
                Console.WriteLine("Kwota:");
                decimal amount = decimal.Parse(Console.ReadLine());

                TransferData data = new TransferData
                {
                    Receiver = numberReceiver,
                    Sender = sender.AccountNumber,
                    Amount = amount
                };

                (isSucces, content) = await api.PostTransfer(data, _tokenSource.Token);
                if (isSucces)
                {
                    TransferResult transferResult = JsonConvert.DeserializeObject<TransferResult>(content);
                    Console.WriteLine("Masz obecnie na koncie " + transferResult.Sender.AccountBalance + " zł.");
                }
                else
                    Console.WriteLine(content);
            }
            else
                Console.WriteLine(content);
        }     
    }
}
