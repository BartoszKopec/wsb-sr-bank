using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClientApp.Services;

namespace ClientApp
{
    class Program
    {
        private static CancellationTokenSource _tokenSource;

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
                Console.WriteLine("Podaj komendę (wpłata, wyplata, przelew, konto, kupno, wyjście):");
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
            //włożenie do wpłatomatu karty, na której jest numer konta wchipowany
            //wpłatomat nosi automatycznie name wpłatomat oraz także ma swój numer konta, który praktycznie oznacza, ile jest pieniędzy w pojemnikach wpłatomatu
            //pobranie z bazy payment, bowiem może nie ma takiego konta
            //do wysłania potrzeba wszystkich danych z TransfertData czyli public decimal Amount, Sender oraz Receiver
            //potem wywołać funcję TransferMoney na BankController

            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Wpłacanie do wpłatomatu.");
            //na karcie jest w chipie zawsze numer konta automatycznie wczytywany, tutaj, siłą rzeczy trzeba podać
            Console.WriteLine("Podanie numeru konta:");
            string number = Console.ReadLine();
            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);

            if (isSucces)
            {
                Payment receiver = JsonConvert.DeserializeObject<Payment>(content);//przekształcenie Json na Payment
                //konto bankomatu/wpłatomatu jest w chipie bankomatu, nie trzeba nigdzie sięgać oraz potwierdzać istnienia
                //Sender i Receiver w TransferDFata to rozumiem numery kont Sendera i receivera, bowiem to zwykłe stringi
                Console.WriteLine();
                Console.WriteLine("Proszę przygotować pieniądze do wpłaty. Ażeby dokonać wpłaty należy nacisnąć przycisk na konsoli wpłatomatu");
                Console.ReadKey();
                Console.WriteLine("Kwota:");
                decimal Amount = decimal.Parse(Console.ReadLine());

                //obiekt TransferMoney
                TransferData data = new TransferData();
                data.Sender = "111111";//przykładowy numer bankomatu/wpłatomatu, zawsze name wpłatomatu to wpłatomat
                data.Receiver = receiver.AccountNumber;
                data.Amount = Amount;

                BankController.TransferMoney(data);


            }
            else
                Console.WriteLine(content);

        }

        static async Task WithdrawMoney()
        {
            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Wypłacanie z bankomatu.");
            //na karcie jest w chipie zawsze numer konta automatycznie wczytywany, tutaj, siłą rzeczy trzeba podać
            Console.WriteLine("Podanie numeru:");
            string number = Console.ReadLine();
            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);

            if (isSucces)
            {
                Payment sender = JsonConvert.DeserializeObject<Payment>(content);//przekształcenie Json na Payment
                //konto bankomatu/wpłatomatu jest w chipie bankomatu, nie trzeba nigdzie sięgać oraz potwierdzać istnienia
                //Sender i Receiver w TransferDFata to rozumiem numery kont Sendera i receivera, bowiem to zwykłe stringi
                Console.WriteLine();
                Console.WriteLine("Ażeby dokonać wypłaty należy nacisnąć przycisk na konsoli bankoomatu");
                Console.ReadKey();
                Console.WriteLine("Kwota:");
                decimal Amount = decimal.Parse(Console.ReadLine());

                //obiekt TransferMoney
                TransferData data = new TransferData();
                data.Receiver = "111112";
                data.Sender = sender.AccountNumber;
                data.Amount = Amount;

                BankController.TransferMoney(data);
                

            }
            else
                Console.WriteLine(content);



        }

        static async Task BuySomething()
        {
            ClientApi api = new ClientApi();
            Console.WriteLine();
            Console.Write("Kupno.");
            //na karcie jest w chipie zawsze numer konta automatycznie wczytywany, tutaj, siłą rzeczy trzeba podać
            Console.WriteLine("Podanie numeru:");
            string number = Console.ReadLine();
            (bool isSucces, string content) = await api.GetPaymentData(number, _tokenSource.Token);

            if (isSucces)
            {
                Payment sender = JsonConvert.DeserializeObject<Payment>(content);//przekształcenie Json na Payment
                //konto sklepu jest zawsze automatycznie dołączane do czytnika, 
                //tutaj będzie w bazie, jednak normalnie odpowiada za to czytnik, który łączy sie z bankiem sprzedającego
                //Sender i Receiver w TransferDFata to rozumiem numery kont Sendera i receivera, bowiem to zwykłe stringi
                Console.WriteLine();
                Console.WriteLine("Proszę zbliżyć kartę do czytnika");
                Console.ReadKey();
                Console.WriteLine("Kwota:");
                decimal Amount = decimal.Parse(Console.ReadLine());

                //obiekt TransferMoney
                TransferData data = new TransferData();
                data.Receiver = "222222";//przykładowe konto sklepu
                data.Sender = sender.AccountNumber;
                data.Amount = Amount;

                BankController.TransferMoney(data);


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
                Payment sender = JsonConvert.DeserializeObject<Payment>(content);//przekształcenie Json na Payment
                //konto sklepu jest zawsze automatycznie dołączane do czytnika, 
                //tutaj będzie w bazie, jednak normalnie odpowiada za to czytnik, który łączy sie z bankiem sprzedającego
                //Sender i Receiver w TransferDFata to rozumiem numery kont Sendera i receivera, bowiem to zwykłe stringi
                Console.WriteLine();
                Console.WriteLine("Proszę podać numer konta odbiorcy");
                string numberReceiver = Console.ReadLine();
                Console.WriteLine("Kwota:");
                decimal Amount = decimal.Parse(Console.ReadLine());

                //obiekt TransferMoney
                TransferData data = new TransferData();
                data.Receiver = numberReceiver;
                data.Sender = sender.AccountNumber;
                data.Amount = Amount;

                BankController.TransferMoney(data);


            }
            else
                Console.WriteLine(content);



        }
     
    }
}
