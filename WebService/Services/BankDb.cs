﻿using LiteDB;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using System;

namespace WebService.Services
{
    public class BankDb
    {
        private readonly List<Payment> queuePayments = new List<Payment>();
        private static readonly LiteDatabase _db= new LiteDatabase("payments.db");
        private readonly ILogger<BankDb> _logger;
        private readonly string _dbPaymentsPath = "payments.db", _dbAccountsPath = "accounts.db";
        private bool isPaymentQueueMovedToDB;

        public BankDb(ILogger<BankDb> logger)
        {
            //_db = new LiteDatabase(_dbPaymentsPath);
            _logger = logger;
            Task.Factory.StartNew(SavingPaymentQueueToDB,
                TaskCreationOptions.LongRunning);
        }

        public BankDb()
        {
            _logger = new Logger<BankDb>(new LoggerFactory());
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
                //using LiteDatabase db = new LiteDatabase($"FileName={_dbPaymentsPath};ReadOnly=true");
                ILiteCollection<Payment> payments = _db.GetCollection<Payment>();
                payment = payments.FindOne(p => p.AccountNumber == accountNumber);
            }
            //_logger.LogInformation("Odczytano poprawnie obiekt klasy Payment");
            return payment;
        }

        public Payment ReadPayment(int id)
        {
            Payment payment;
            lock (queuePayments)
            {
                payment = queuePayments.Find(element => element.Id == id);
            }
            if (payment is null)
            {
                using LiteDatabase db = new LiteDatabase(_dbPaymentsPath);
                ILiteCollection<Payment> payments = db.GetCollection<Payment>();
                payment = payments.FindOne(p => p.Id == id);
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
            _logger.LogInformation("Poprawnie zaktualizowano Payment");
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
            using LiteDatabase paymentDb = new LiteDatabase(_dbPaymentsPath);

            ILiteCollection<Account> accounts = db.GetCollection<Account>();
            ILiteCollection<Payment> payments = paymentDb.GetCollection<Payment>();

            payments.Delete(id);
            bool status = accounts.Delete(id);

            _logger.LogInformation("Usunięto poprawnie obiekt klasy Account");

            return status;
        }

        public int AddSpecialAccount(string accountNumberName)
        {
            using LiteDatabase accountDb = new LiteDatabase(_dbAccountsPath);
            using LiteDatabase paymentDb = new LiteDatabase(_dbPaymentsPath);

            ILiteCollection<Account> accounts = accountDb.GetCollection<Account>();
            ILiteCollection<Payment> payments = paymentDb.GetCollection<Payment>();

            int id = accounts.Insert(new Account
            {
                FirstName = accountNumberName
            });
            payments.Insert(new Payment
            {
                Id = id,
                AccountNumber = accountNumberName,
                AccountBalance = 10000m
            });

            return id;
        }


        public int AddAccount(Account account)
        {
            using LiteDatabase accountDb = new LiteDatabase(_dbAccountsPath);
            using LiteDatabase paymentDb = new LiteDatabase(_dbPaymentsPath);

            ILiteCollection<Account> accounts = accountDb.GetCollection<Account>();
            ILiteCollection<Payment> payments = paymentDb.GetCollection<Payment>();

            string newPaymentAccountNumber = string.Empty;
            do
            {
                newPaymentAccountNumber = RandomGenerator.GetAccountNumber();
            } while (payments.Exists(p => p.AccountNumber == newPaymentAccountNumber));

            int id = accounts.Insert(account);
            payments.Insert(new Payment
            {
                Id = id,
                AccountNumber = newPaymentAccountNumber,
                AccountBalance = 100m
            });

            _logger.LogInformation("Dodano poprawnie obiekt klasy Account");

            return id;
        }

        public bool AddCard(int id)
        {
            using LiteDatabase accountDb = new LiteDatabase(_dbAccountsPath);
            ILiteCollection<Account> accounts = accountDb.GetCollection<Account>();
            Account account = accounts.FindById(id);

            if (account is null)
                return false; 

            string newCardNumber;
            do
            {
                newCardNumber = RandomGenerator.GetCardNumber();
            } while (accounts.FindAll().Any(a => a.Cards.Find(c => c.NumberOfCard == newCardNumber) != null));


            account.Cards.Add(new Card
            {
                NumberOfCard = newCardNumber,
                SafeCode = RandomGenerator.GetCardSafeCode(),
                Month = (byte)DateTime.Now.Month,
                Year = (byte)((DateTime.Now.Year % 100) + 3)
            });

            bool status = accounts.Update(account);
            return status;
        }

        public bool RemoveCard(int id, string cardNumber)
        {
            using LiteDatabase accountDb = new LiteDatabase(_dbAccountsPath);
            ILiteCollection<Account> accounts = accountDb.GetCollection<Account>();
            Account account = accounts.FindById(id);

            if (account is null)
                return false;

            int removedCount = account.Cards.RemoveAll(c => c.NumberOfCard == cardNumber);

            bool status = accounts.Update(account);

            return removedCount == 1 && status;
        }

        public bool UpdateAccount(Account account)
        {
            using LiteDatabase db = new LiteDatabase(_dbAccountsPath);

            ILiteCollection<Account> accounts = db.GetCollection<Account>();

            bool status = accounts.Update(account);

            _logger.LogInformation("Zaktualizowano poprawnie obiekt klasy Account");

            return status;
        }

        private void SavingPaymentQueueToDB()
        {
            while (true)
            {
                if (queuePayments.Count > 0 && !isPaymentQueueMovedToDB)
                {
                    isPaymentQueueMovedToDB = true;
                    //using LiteDatabase db = new LiteDatabase(_dbPaymentsPath);
                    lock (_db)
                    {
                        ILiteCollection<Payment> payments = _db.GetCollection<Payment>();
                        lock (queuePayments)
                        {
                            payments.Upsert(queuePayments);
                            queuePayments.Clear();
                        }
                    }
                    isPaymentQueueMovedToDB = false;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
