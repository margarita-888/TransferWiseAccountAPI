using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TransferWiseCommon;

namespace WebhookTest
{
    internal class Program
    {
        private const string ALGORITHM_1 = "SHA1WITHRSA";

        private static void Main(string[] args)
        {
            Console.WriteLine("Webhook Test. Would like to go ahead? Please answer Y or N:");
            var answer = Console.ReadLine();

            if (answer.ToLower() == "y")
                SendBalancesCreditWebhook().Wait();
        }

        private async static Task SendBalancesCreditWebhook()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:64480");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payloadString = @"{""data"":{""resource"":{""id"":111,""type"":""balance-account"",""profile_id"":222},""amount"":1.23,""currency"":""EUR"",""transaction_type"":""credit"",""post_transaction_balance_amount"":2.34,""occurred_at"":""2020-07-06T22:46:24.766Z""},""subscription_id"":""01234567-89ab-cdef-0123-456789abcdef"",""event_type"":""balances#credit"",""schema_version"":""2.0.0"",""sent_at"":""2020-07-06T22:46:24.764Z""}";

            try
            {
                // Sign json payload with a private key
                var signatureHeaderValue = SignatureHelper.SignWithPrivateKey(payloadString, ALGORITHM_1);
                if (string.IsNullOrEmpty(signatureHeaderValue))
                {
                    Console.WriteLine("WebhookTest::SendBalancesCredit. Signing jsonString with a private key has failed. Unable to continue.");
                    return;
                }
                Console.WriteLine($"\nX-Signature: {signatureHeaderValue}");

                var uri = client.BaseAddress + "api/receivedeposit";
                Console.WriteLine($"\nRequest url: {uri}");

                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Headers.Add("X-Signature", signatureHeaderValue);

                var stringContent = new StringContent(payloadString, Encoding.UTF8, "application/json");
                request.Content = stringContent;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"WebhookTest::SendBalancesCredit. Response status code: {response.StatusCode.ToString()}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"WebhookTest::SendBalancesCredit. Exception {ex.Message}. {ex.StackTrace}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebhookTest::SendBalancesCredit. Exception {ex.Message}. {ex.StackTrace}");
                return;
            }
        }
    }
}