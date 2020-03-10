using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Apis;
using Core.Models;
using Newtonsoft.Json;

namespace WorkerApp.Services
{
    class WorkerApi : BaseApi
    {
        public WorkerApi() : base()
        {
        }

        public async Task<(bool, string)> InsertAccount(Account account, CancellationToken token) =>
            await PostAsync("/accounts/insert", JsonConvert.SerializeObject(account), token);

        public async Task<(bool, string)> GetAccountData(int id, CancellationToken token) =>
            await GetAsync($"/accounts/get/{id}", token);

        public async Task<(bool, string)> GetPaymentData(int id, CancellationToken token) =>
            await GetAsync($"/bank/payment?id={id}", token);


        public async Task<(bool, string)> GetAllAccounts(CancellationToken token) =>
            await GetAsync("/accounts/getall", token);
        
        public async Task<(bool, string)> UpdateAccountData(Account account, CancellationToken token) =>
            await PutAsync("/accounts/update", JsonConvert.SerializeObject(account), token);
        
        public async Task<(bool, string)> DeleteAccount(int id, CancellationToken token) =>
            await DeleteAsync($"/accounts/delete/{id}", token);

        public async Task<(bool, string)> AddCardToAccount(int id, CancellationToken token) =>
            await PutAsync($"/accounts/{id}/addcard", "", token);

        public async Task<(bool, string)> RemoveCardToAccount(int id, string cardNumber, CancellationToken token) =>
            await DeleteAsync($"/accounts/{id}/removecard/{cardNumber}", token);

    }
}
