using Core.Apis;
using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    class ClientApi : BaseApi
    {
        public ClientApi() : base()
        {
        }

        public async Task<(bool, string)> PostTransfer(TransferData data, CancellationToken token) =>
            await PostAsync("/bank/transfer", JsonConvert.SerializeObject(data), token);

        public async Task<(bool, string)> GetAccountData(int id, CancellationToken token) =>
            await GetAsync($"/account/get/{id}", token);

    }
}
