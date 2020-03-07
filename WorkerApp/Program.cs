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

        static async Task Main(string[] args)
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
                    case "wyjście": isEnd = true; break;
                    case "likwidacja": await DeleteAccount(); break;
                    case "edycja": await UpgradeAccount(); break;
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
            
            //zdobycie konta z bazy za pomocą podanego ID
            Console.WriteLine("Edycja konta. Podaj ID konta");
            int id = int.Parse(Console.ReadLine());
            Account TheAccount = new Account();
            (bool isSucces, string content)=await api.GetAccountData(id, _tokenSource.Token);

            if (isSucces)
            { 
                Console.WriteLine("-------------------");
                //....................
                //Zaktualizowanie pól tego pobranego konta
                /*
                0. to wszystko w pętli
                1. pytamy usera co chce zmienić (imię, nazwisko, pesel, zakończ)
                2. user wpisuje co chce zmienić
                3. w switchu sprawdzamy co chce zmienić i wykonujemy odpowiednią akcję
                   3.1 akcja "zakończ" przerwie pętlę i wyśle aktualizacje na serwer
                   3.2 po wykonaniu innych akcji niż "zakończ" algorytm wróci do pkt 1
                */
                bool isEnd = false;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Podaj co chcesz zmodyfikować (imię, nazwisko, PESEL, zakończ):");
                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "imię":
                            {
                                Console.WriteLine("Podaj imię:");
                                TheAccount.FirstName = Console.ReadLine();
                                break;
                            }

                        case "nazwisko":
                            {
                                Console.WriteLine("Podaj nazwisko:");
                                TheAccount.LastName = Console.ReadLine();
                                break;
                            }

                        case "PESEL":
                            {
                                Console.WriteLine("Podaj PESEL:");
                                TheAccount.Pesel = Console.ReadLine();
                                break;
                            }

                        case "zakończ":
                            {
                                (isSucces, content) = await api.UpdateAccountData(TheAccount, _tokenSource.Token);
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




                //....................
                //wysłanie aktualizacji na serwer

            }
            else
            {
                Console.WriteLine(content);
            }
        }
    }
}
