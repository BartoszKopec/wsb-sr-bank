using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkerApp.Services;

namespace WorkerApp
{
    class Program
    {
        private static CancellationTokenSource _tokenSource;

        static async Task Main()
        {
            _tokenSource = new CancellationTokenSource();
            Console.WriteLine("Aplikacja pracownika banku");
            await DrawUI();
        }

        static async Task DrawUI()
        {
            bool isEnd = false;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Podaj komendę (dodanie, likwidacja, edycja, dane, wszystkie, wyjście):");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "dodanie": await AddAccount(); break;
                    case "wszystkie": await ShowAccounts(); break;
                    case "dane": await ShowOneAccount(); break;
                    case "wyjście": isEnd = true; break;
                    case "likwidacja": await DeleteAccount(); break;
                    case "edycja": await UpdateAccountData(); break;
                    default:
                        break;
                }
            } while (!isEnd);
        }

        static async Task AddAccount()
        {
            Account newAccount = new Account();

            Console.WriteLine();
            Console.WriteLine("Dodanie konta:");
            Console.WriteLine("Podaj imię:");
            newAccount.FirstName = Console.ReadLine();
            Console.WriteLine("Podaj nazwisko:");
            newAccount.LastName = Console.ReadLine();
            Console.WriteLine("Podaj PESEL:");
            newAccount.Pesel = Console.ReadLine();
            Console.WriteLine("Wprowadzam konto do bazy");

            WorkerApi api = new WorkerApi();
            (bool isSucces, string content) = await api.InsertAccount(newAccount, _tokenSource.Token);
            if (isSucces)
            {
                int id = JsonConvert.DeserializeObject<Account>(content).Id;
                Console.WriteLine($"Pomyślnie dodano konto. Id nowego konta: {id}");
            }
            else
            {
                Console.WriteLine(content);
            }
        }

        static async Task ShowAccounts()
        {
            WorkerApi api = new WorkerApi();

            Console.WriteLine();
            Console.WriteLine("Wyświetlenie wszystkich kont:");

            (bool isSuccess, string content) = await api.GetAllAccounts(_tokenSource.Token);
            if (isSuccess)
            {
                List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(content);
                Console.WriteLine("-------------------");
                accounts.ForEach(account =>
                {
                    string output = string.Format(
                        "Id: {0}, Imię: {1}, Nazwisko: {2}, Pesel: {3}",
                        account.Id, account.FirstName, account.LastName, account.Pesel);
                    Console.WriteLine(output);
                });
                Console.WriteLine("-------------------");
            }
            else
                Console.WriteLine(content);
        }

        static async Task ShowOneAccount()
        {
            WorkerApi api = new WorkerApi();
            Console.WriteLine();
            Console.WriteLine("Pokazywanie zawartości konta konta:");
            Console.WriteLine("Podaj Id konta:");
            int id = int.Parse(Console.ReadLine());

            (bool isSucces, string content)=await api.GetAccountData(id, _tokenSource.Token);
            
            if (isSucces)
            {
                Account account = JsonConvert.DeserializeObject<Account>(content);
                Console.WriteLine();
                Console.WriteLine(string.Format(
                    "ID danego konta to: {0}\nImię właściciela konta to: {1}\nNazwisko właściciela konta to: {2}\nPESEL właściciela konta to: {3}", 
                    account.Id, account.FirstName, account.LastName, account.Pesel
                    ));
                Console.WriteLine("Numery kart:");
                foreach(var card in account.Cards)
                {
                    Console.WriteLine(card.NumberOfCard);
                }
            }
            else
                Console.WriteLine(content);

        }


        static async Task DeleteAccount()
        {
            WorkerApi api = new WorkerApi();
            Console.WriteLine();
            Console.WriteLine("Likwidacja konta:");
            Console.WriteLine("Podaj Id konta:");
            int id = int.Parse(Console.ReadLine());

            (bool isSuccess, string content) = await api.DeleteAccount(id, _tokenSource.Token);
            if (isSuccess)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine($"Pomyślnie usunięto klienta mającego następujący numer: {id}");
                Console.WriteLine("-------------------");
            }
            else
            {
                Console.WriteLine(content);
            }

        }

        static async Task UpdateAccountData()
        {
            WorkerApi api = new WorkerApi();
            Console.WriteLine();
            
            Console.WriteLine("Edycja konta. Podaj ID konta");
            int id = int.Parse(Console.ReadLine());
            (bool isSucces, string content)=await api.GetAccountData(id, _tokenSource.Token);
            
            if (isSucces)
            {
                Account account = JsonConvert.DeserializeObject<Account>(content);
                Console.WriteLine("-------------------");
                bool isEnd = false;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Podaj co chcesz zmodyfikować (imię, nazwisko, PESEL, karty, zakończ):");
                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "karty":
                            {
                                Console.WriteLine("Dodać lub usunąć?");
                                string cardCommand = Console.ReadLine();
                                if(cardCommand.ToLower() == "dodać")
                                {
                                    (isSucces, content) = await api.AddCardToAccount(account.Id, _tokenSource.Token);
                                    if(!isSucces)
                                        Console.WriteLine(content);
                                }
                                else if(cardCommand.ToLower() == "usunąć")
                                {
                                    Console.WriteLine("Podaj nr karty do usunięcia:");
                                    string cardNumber = Console.ReadLine();
                                    (isSucces, content) = await api.RemoveCardToAccount(
                                        account.Id, 
                                        cardNumber, 
                                        _tokenSource.Token);
                                    if (!isSucces)
                                        Console.WriteLine(content);
                                }
                                else
                                    Console.WriteLine("Błędna komenda");
                            }break;
                        case "imię":
                            {
                                Console.WriteLine("Podaj imię:");
                                account.FirstName = Console.ReadLine();
                                break;
                            }

                        case "nazwisko":
                            {
                                Console.WriteLine("Podaj nazwisko:");
                                account.LastName = Console.ReadLine();
                                break;
                            }

                        case "PESEL":
                            {
                                Console.WriteLine("Podaj PESEL:");
                                account.Pesel = Console.ReadLine();
                                break;
                            }

                        case "zakończ":
                            {
                                (isSucces, content) = await api.UpdateAccountData(account, _tokenSource.Token);
                                if (!isSucces)
                                {
                                    Console.WriteLine(content);
                                }
                                Console.WriteLine("Poprawiono konto o numerze ID " + id);
                                isEnd = true; 
                                break;
                            }
                        default:
                            break;
                    }
                } while (!isEnd);
            }
            else
            {
                Console.WriteLine(content);
            }
        }
    }
}
