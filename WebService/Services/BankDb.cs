using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebService.Models;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace WebService.Services
{
    public class BankDb
    {
        private readonly List<Payment> queuePayments = new List<Payment>();
        private readonly ILogger<BankDb> _logger;
        private readonly string _dbPaymentsPath = "payments.db", _dbAccountsPath = "accouts.db";
        private bool isPaymentQueueMovedToDB;

        public BankDb(ILogger<BankDb> logger)
        {
            _logger = logger;
            TemporaryFillDatabase();
            Task.Run(()=>
            {
                _logger.LogInformation("Uruchomiono wątek w tle");
                while (true)
                {
                    Task.Delay(500);
                    if (queuePayments.Count > 0 && !isPaymentQueueMovedToDB) 
                    {
                        SavePaymentQueueToFile();
                    }
                }
            });
            
        }
        
        public Payment ReadPayment(string accountNumber)
        {
            Payment payment;
            lock (queuePayments) 
            {
                payment = queuePayments.Find(element => element.AccountNumber == accountNumber);
            }
            if (payment is null)
            {
                using LiteDatabase db = new LiteDatabase(_dbPaymentsPath);
                ILiteCollection<Payment> payments = db.GetCollection<Payment>();

                payment = payments.FindOne(p => p.AccountNumber == accountNumber);
            }
            _logger.LogInformation("Odczytano poprawnie obiekt klasy Payment");
            return payment;
        }

        public void UpdatePayment(Payment newPayment)
        {
            bool isPaymentFound = false;
            lock (queuePayments) //zajęcie tej kolekcji przez główny wątek żeby wątek dodania do bazy poczekał
            {
                for (int i = 0; i < queuePayments.Count; i++)
                {
                    if (queuePayments[i].AccountNumber == newPayment.AccountNumber)
                    {
                        isPaymentFound = true;
                        queuePayments[i].AccountBalance = newPayment.AccountBalance;
                        break;
                    }
                }
                if (!isPaymentFound)
                    queuePayments.Add(newPayment);
            }
            _logger.LogInformation("Poprawnie zaktualozowano Payment");
        }

        public Account ReadAccount(int id)
        {
            Account account;

            using LiteDatabase db = new LiteDatabase(_dbAccountsPath);

            ILiteCollection<Account> accounts = db.GetCollection<Account>();

            account = accounts.FindOne(a => a.Id == id);

            _logger.LogInformation("Odczytano poprawnie obiekt klasy Account");

            return account;
        }

        public List<Account> ReadAccounts()
        {
            List<Account> accounts;

            using LiteDatabase db = new LiteDatabase(_dbAccountsPath);

            ILiteCollection<Account> accountsCollection = db.GetCollection<Account>();

            accounts = accountsCollection.FindAll().ToList();

            _logger.LogInformation("Odczytano poprawnie listę obiektów klasy Account");

            return accounts;
        }

        public bool DeleteAccount(int id)
        {
            using LiteDatabase db = new LiteDatabase(_dbAccountsPath);

            ILiteCollection<Account> accounts = db.GetCollection<Account>();

            Account account = accounts.FindOne(a => a.Id == id);

            bool status = accounts.Delete(account.Id);

            _logger.LogInformation("Usunięto poprawnie obiekt klasy Account");
            
            return status;
        }

        public int AddAccount(Account account)
        {
            using LiteDatabase db = new LiteDatabase(_dbAccountsPath);

            ILiteCollection<Account> accounts = db.GetCollection<Account>();

            int id = accounts.Insert(account);

            _logger.LogInformation("Dodano poprawnie obiekt klasy Account");

            return id;
        }

        public bool UpdateAccount(Account account)
        {
            using LiteDatabase db = new LiteDatabase(_dbAccountsPath);

            ILiteCollection<Account> accounts = db.GetCollection<Account>();

            bool status = accounts.Update(account);

            _logger.LogInformation("Zaktualizowano poprawnie obiekt klasy Account");

            return status;
        }

        private void TemporaryFillDatabase()
        {
            using LiteDatabase db = new LiteDatabase(_dbPaymentsPath);
            ILiteCollection<Payment> payments = db.GetCollection<Payment>();
            payments.Insert(new Payment
            {
                AccountNumber = "1234",
                AccountBalance = 100M
            });
            payments.Insert(new Payment
            {
                AccountNumber = "qwer",
                AccountBalance = 100M
            });
        }

        private void SavePaymentQueueToFile()
        {
            _logger.LogInformation("Wstawienie wypełnionej kolejki do bazy");
            isPaymentQueueMovedToDB = true;
            using LiteDatabase db = new LiteDatabase(_dbPaymentsPath);
            ILiteCollection<Payment> payments = db.GetCollection<Payment>();
            lock (queuePayments)
            {
                queuePayments.ForEach(element =>
                {
                    if (payments.Exists(p => p.AccountNumber == element.AccountNumber))
                        payments.Update(element);
                    else
                        payments.Insert(element);
                });
                queuePayments.Clear();
            }
            isPaymentQueueMovedToDB = false;
            _logger.LogInformation("Poprawnie zapisano kolejkę do bazy");
        }
    }
}
