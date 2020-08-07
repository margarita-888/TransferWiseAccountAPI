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
        private static void Main(string[] args)
        {
            Console.WriteLine("Webhook Test.");
            Console.WriteLine("Would like to go ahead? Y or N:");
            var answer = Console.ReadLine();
            if (answer == "y")
            {
                SendBalanceCreditEvent().Wait();
                Console.WriteLine("Run test again? Y or N:");
                answer = Console.ReadLine();
            }

            while (answer != "n")
            {
                SendBalanceCreditEvent().Wait();
                Console.WriteLine("Run test again? Y or N:");
                answer = Console.ReadLine();
            }
        }

        private async static Task SendBalanceCreditEvent()
        {
            DateTime beginProcessingTimeUTC;
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:64480")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = @"{""data"":{""resource"":{""id"":111,""type"":""balance-account"",""profile_id"":222},""amount"":1.23,""currency"":""EUR"",""transaction_type"":""credit"",""post_transaction_balance_amount"":2.34,""occurred_at"":""2020-07-06T22:46:24.766Z""},""subscription_id"":""01234567-89ab-cdef-0123-456789abcdef"",""event_type"":""balances#credit"",""schema_version"":""2.0.0"",""sent_at"":""2020-07-06T22:46:24.764Z""}";

            try
            {
                // Sign json payload with a private key
                var signatureHeaderValue = SignatureHelper.SignWithPrivateKey(json);
                if (string.IsNullOrEmpty(signatureHeaderValue))
                {
                    Console.WriteLine("WebhookTest::SendBalanceCreditEvent. Error. Signing jsonString with a private key has failed. Unable to continue.");
                    return;
                }
                Console.WriteLine($"\nX-Signature-SHA256: {signatureHeaderValue}");

                var uri = client.BaseAddress + "api/transferwiseevents/balancecredit";
                Console.WriteLine($"\nRequest url: {uri}");

                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Headers.Add("X-Signature-SHA256", signatureHeaderValue);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;

                beginProcessingTimeUTC = DateTime.UtcNow;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var totalProcessingTime = (DateTime.UtcNow - beginProcessingTimeUTC).TotalMilliseconds.ToString("###,###,##0");
                Console.WriteLine($"Total processing time is {totalProcessingTime} milliseconds.");
                Console.WriteLine($"Response status code: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"WebhookTest::SendBalanceCreditEvent. Exception. {ex.Message}. {ex.StackTrace}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebhookTest::SendBalanceCreditEvent. Exception. {ex.Message}. {ex.StackTrace}");
                return;
            }
        }
    }
}