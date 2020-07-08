using System;
using System.Threading.Tasks;

namespace TransferWiseClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            TransferWiseHttpClient client = new TransferWiseHttpClient();
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