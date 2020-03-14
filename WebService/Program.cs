using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebService.Services;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BankDb bankDb = new BankDb();
            if(bankDb.ReadPayment("wplatomat") is null)
                bankDb.AddSpecialAccount("wplatomat");
            if (bankDb.ReadPayment("bankomat") is null)
                bankDb.AddSpecialAccount("bankomat");
            if (bankDb.ReadPayment("sklep") is null)
                bankDb.AddSpecialAccount("sklep");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
