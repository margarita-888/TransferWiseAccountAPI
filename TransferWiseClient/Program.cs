using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseClient
{
    internal class Program
    {
        private static IConfigurationRoot configuration;

        private static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(environment) || environment.ToLower() == "development";
            var builder = new ConfigurationBuilder();

            // only add secrets in development
            if (isDevelopment)
            {
                builder.AddUserSecrets<Secrets>();
            }

            configuration = builder.Build();

            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            TransferWiseHttpClient client = new TransferWiseHttpClient(configuration);
            try
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine("\nTransferWise Profiles");
                var profileId = await client.GetTransferwiseBusinessProfileId();
                if (profileId <= 0)
                {
                    return;
                }

                Console.WriteLine("===============================================================================");
                Console.WriteLine("\nTransferWise Balances");
                var balances = await client.GetTransferwiseAccountBalances(profileId);
                if (balances == null)
                {
                    return;
                }

                Console.WriteLine("===============================================================================");
                Console.WriteLine("\nTransferWise Statements");
                var statements = await client.GetTransferwiseAccountStatements(profileId, balances);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TransferWiseClient::Program::RunAsync. Exception, { ex.Message}. { ex.StackTrace}.");
            }
        }
    }
}