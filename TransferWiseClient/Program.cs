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
            var env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";
            var builder = new ConfigurationBuilder();

            // only add secrets in development
            // for production purposes add to App Settings
            if (isDevelopment)
            {
                builder.AddUserSecrets<Secrets>();
            }

            configuration = builder.Build();

            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            if (string.IsNullOrEmpty(configuration.GetSection("ACCESS_TOKEN").Value))
            {
                Console.WriteLine($"TransferWiseClient::Program::RunAsync. Error. ACCESS_TOKEN not found. Unable to proceed.");
                return;
            }

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