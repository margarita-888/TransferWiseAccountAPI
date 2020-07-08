using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using TransferWiseCommon;

namespace TransferWiseClient
{
    public class TransferWiseHttpClient
    {
        //private const string ACCESS_TOKEN = "baa5a69f-0b8f-4c6f-b96b-21bf3ac86c1c";
        //private const string ACCESS_TOKEN = "eda86ae4-2717-459a-b740-99a191f5083c"; // Sandbox full access key

        private const string ACCESS_TOKEN = "b02e8fc4-72ca-4c78-bea9-db0a39e503ff";   // Sandbox limited access key
        private const string ALGORITHM_256 = "SHA256WITHRSA";

        private readonly HttpClient client = new HttpClient();

        public TransferWiseHttpClient()
        {
            client.BaseAddress = new Uri("https://api.sandbox.transferwise.tech/"); // Sandbox Uri
                                                                                    // Live BaseAddress is "https://api.transferwise.com/"
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);
        }

        // GET https://api.sandbox.transferwise.tech/v1/profiles
        public async Task<int> GetTransferwiseBusinessProfileId()
        {
            try
            {
                Console.WriteLine("Sending GET request to https://api.sandbox.transferwise.tech/v1/profiles");

                HttpResponseMessage response = await client.GetAsync("v1/profiles");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountBalances. Response StatusCode was {response.StatusCode.ToString()}.");
                    return -1;
                }

                var profiles = await response.Content.ReadAsAsync<List<ProfileDTO>>();
                if (profiles == null)
                {
                    Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseBusinessProfileId. Returned profiles were null.");
                    return -1;
                }
                Console.WriteLine("\nReceived TransferWise profiles:\n" + JsonConvert.SerializeObject(profiles, new JsonSerializerSettings()
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }));

                var businessProfileId = profiles.Where(p => p.Type == "business").Select(p => p.Id).FirstOrDefault();

                Console.WriteLine($"TransferWise business profileId is {businessProfileId.ToString()}");
                return businessProfileId;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"TransferWiseHttpClient::GetTransferwiseBusinessProfileId. Exception {ex.Message}. {ex.StackTrace}");
                return -1;
            }
        }

        // Get available balances for all activated currencies in your account
        // GET https://api.sandbox.transferwise.tech/v1/borderless-accounts?profileId={profileId}
        public async Task<List<BalanceDTO>> GetTransferwiseAccountBalances(int profileId)
        {
            if (profileId <= 0)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountBalances. ProfileId must be a whole number > 0.");
                return null;
            }

            Console.WriteLine($"\nSending GET request to https://api.sandbox.transferwise.tech/v1/borderless-accounts?profileId={profileId}.");

            var builder = new UriBuilder(client.BaseAddress + "v1/borderless-accounts?");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["profileId"] = profileId.ToString();
            builder.Query = query.ToString();
            var uri = builder.ToString();

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountBalances. Response StatusCode was {response.StatusCode.ToString()}.");
                    return null;
                }

                var balances = await response.Content.ReadAsAsync<List<BalanceDTO>>();

                if (balances == null)
                {
                    Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountBalances. Account balances were null.");
                    return null;
                }

                Console.WriteLine("\nReceived TransferWise account balances:\n" + JsonConvert.SerializeObject(balances, new JsonSerializerSettings()
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }));

                return balances;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"TransferWiseHttpClient::GetTransferwiseAccountBalances. Exception {ex.Message}. {ex.StackTrace}");
                return null;
            }
        }

        #region "TransferWise Account Statement"

        // GET https://api.sandbox.transferwise.tech/v3/profiles/{profileId}/borderless-accounts/{borderlessAccountId}/statement.json?currency=EUR&intervalStart=2018-03-01T00:00:00.000Z&intervalEnd=2018-03-15T23:59:59.999Z
        //  Follow TransferWise Strong Authentication algorithm
        //  When Strong Authentication is required, 403 Forbidden HTTP status code Is returned together with a one-time token (OTT) value which needs to be signed And the resulting signature included in the retry of the original request.
        //  TransferWise use a digital signature scheme based on public-key cryptography. It involves creating a signature using a private key on the client side And verifying the signature authenticity on the server side using the corresponding public key the client has uploaded.
        //  To call the endpoints requiring additional authentication

        //  Create a key pair consisting of a public And a private key
        //  Upload the public key to TransferWise
        //  Set up response handling to retry with the signed OTT
        //  Creating the key pair

        //  Keys can be generated with the OpenSSL toolkit:
        //  $ openssl genrsa -out private.pem 2048
        //  $ openssl rsa -pubout -in private.pem -out public.pem
        //  The following requirements apply
        //  The cryptographic algorithm has to be RSA
        //  The key length has to be at least 2048 bits
        //  The public key should be stored in PEM file format using a .pem file extension

        public async Task<List<StatementDTO>> GetTransferwiseAccountStatements(int profileId, List<BalanceDTO> balances)
        {
            if (profileId <= 0)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountStatement. ProfileId must be a whole number > 0.");
                return null;
            }

            if (balances == null)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountStatement. Unable to proceed. Returned account balances were null.");
                return null;
            }

            // Normally each Borderless account would have a few balance accounts to operate in different currencies. We need to get statements for each currency
            var borderlessAccountIds = balances.Select(b => b.Id).ToList();
            if (borderlessAccountIds == null || borderlessAccountIds.Count <= 0)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountStatement. Unable to proceed. TransferWise account Ids were NOT found.");
                return null;
            }

            var borderlessAccounts = balances.Select(b => b.Balances).ToList();
            if (borderlessAccounts == null)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountStatement. Unable to proceed. TransferWise account balances were NOT found.");
                return null;
            }

            var currencies = new List<string>();
            foreach (var borderlessAccount in borderlessAccounts)
            {
                foreach (var balanceAccount in borderlessAccount)
                {
                    currencies.Add(balanceAccount.Currency);
                }
            }
            currencies = currencies.Distinct().ToList();

            if (currencies.Count <= 0)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountStatement. Unable to proceed. TransferWise account currencies were NOT found.");
                return null;
            }

            var statements = new List<StatementDTO>();

            // Set up start and end dates to retrieve statements for the last 3 days
            var startDateZuluFormat = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd'T'HH:mm:ss.fff") + "Z";
            var endDateZuluFormat = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff") + "Z";

            Console.WriteLine($"\nStatement start date UTC: {DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd'T'HH:mm:ss.fff")}");
            Console.WriteLine($"\nStatement end date UTC: {DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff")}");

            try
            {
                foreach (var borderlessAccountId in borderlessAccountIds)
                {
                    foreach (var currency in currencies)
                    {
                        var builder = new UriBuilder(client.BaseAddress + $"v3/profiles/{profileId}/borderless-accounts/{borderlessAccountId}/statement.json?");
                        builder.Port = -1;
                        var query = HttpUtility.ParseQueryString(builder.Query);
                        query["currency"] = currency;
                        query["intervalStart"] = startDateZuluFormat;
                        query["intervalEnd"] = endDateZuluFormat;
                        builder.Query = query.ToString();
                        var uri = builder.ToString();

                        Console.WriteLine($"\nStatement currency: {currency}");
                        Console.WriteLine($"\nSending GET request to https://api.sandbox.transferwise.tech/v3/profiles/{profileId}/borderless-accounts/{borderlessAccountId}/statement.json?currency={currency}&intervalStart={startDateZuluFormat}&intervalEnd={endDateZuluFormat}.");

                        var accountStatement = await GetStatement(uri);
                        if (accountStatement == null)
                        {
                            Console.WriteLine($"\nTransferWiseHttpClient::GetTransferwiseAccountStatement. TransferWise statement for currency {currency} was NOT found.");
                        }
                        else
                        {
                            statements.Add(accountStatement);
                        }
                    }
                }

                return statements;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TransferWiseHttpClient::GetTransferwiseAccountBalances. Exception {ex.Message}. {ex.StackTrace}");
                return null;
            }
        }

        private async Task<StatementDTO> GetStatement(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                Console.WriteLine($"TransferWiseHttpClient::GetStatement. Parameter uri must be provided. Unable to proceed.");
                return null;
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                Console.WriteLine($"\nResponse StatusCode was {response.StatusCode.ToString()}.");

                if (response.StatusCode == HttpStatusCode.Forbidden) // 403
                {
                    return await Process403StatementResponse(uri, response);
                }
                else if (response.StatusCode == HttpStatusCode.OK) // 200
                {
                    return await Process200StatementResponse(response);
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"TransferWiseHttpClient::GetStatement. Exception {ex.Message}. {ex.StackTrace}");
                return null;
            }
        }

        private async Task<StatementDTO> Process403StatementResponse(string uri, HttpResponseMessage response)
        {
            if (string.IsNullOrEmpty(uri))
            {
                Console.WriteLine($"TransferWiseHttpClient::Process403StatementResponse. Unable to proceed. Parameter uri was null or an empty string. ");
                return null;
            }

            if (response == null)
            {
                Console.WriteLine($"TransferWiseHttpClient::Process403StatementResponse. Unable to proceed. Parameter response was null.");
                return null;
            }

            //  Retrieve One-Time Token (OTT) from x-2fa-approval header
            //  Sign OTT with a private key stored as private.pem file and repeat request with OTT sent as x-2fa-approval header and signature sent as X-Signature header
            try
            {
                var headers = response.Headers.ToList();

                var x2FAApprovalHeader = headers.Find(h => h.Key.Equals("x-2fa-approval"));
                if (x2FAApprovalHeader.Value == null || String.IsNullOrEmpty(x2FAApprovalHeader.Value.FirstOrDefault()))
                {
                    Console.WriteLine("TransferWiseAccountAPI::Process403StatementResponse. Exception, Unable to find 'x-2fa-approval' header in TransferWise response.");
                    return null;
                }

                // Retrieve One-Time Token
                var oTT = x2FAApprovalHeader.Value.FirstOrDefault();
                Console.WriteLine($"\nOTT received in 403 (Forbidden) response as 'x-2fa-approval' header: {oTT}");

                // Sign OTT with a private key
                var signatureHeaderValue = SignatureHelper.SignWithPrivateKey(oTT, ALGORITHM_256);
                Console.WriteLine($"\nX-Signature: {signatureHeaderValue}");

                if (string.IsNullOrEmpty(signatureHeaderValue))
                {
                    Console.WriteLine("TransferWiseAccountAPI::Process403StatementResponse. Exception, Unable to find 'x-2fa-approval' header in TransferWise response.");
                    return null;
                }

                // Repeate request to TransferWise with OTT and Signature headers
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("x-2fa-approval", oTT);
                request.Headers.Add("X-Signature", signatureHeaderValue);

                response = await client.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"\nTransferWiseHttpClient::Process403StatementResponse. Response StatusCode was {response.StatusCode.ToString()}.");
                    return null;
                }

                // process 200 OK response
                return await Process200StatementResponse(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nTransferWiseHttpClient::Process403StatementResponse. Exception {ex.Message}. {ex.StackTrace}.");
                return null;
            }
        }

        private async Task<StatementDTO> Process200StatementResponse(HttpResponseMessage response)
        {
            if (response == null)
            {
                Console.WriteLine($"TransferWiseHttpClient::Process200StatementResponse. Unable to proceed. Parameter response was null.");
                return null;
            }

            try
            {
                Console.WriteLine($"Retrieving account statement from 200 response.");
                var statement = await response.Content.ReadAsAsync<StatementDTO>();
                if (statement == null)
                {
                    Console.WriteLine($"\nTransferWiseHttpClient::Process200StatementResponse. Returned statement was null.");
                    return null;
                }

                Console.WriteLine("\nReceived TransferWise statement:\n" + JsonConvert.SerializeObject(statement, new JsonSerializerSettings()
                {
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }));

                return statement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TransferWiseHttpClient::Process200StatementResponse. Exception {ex.Message}. {ex.StackTrace}");
                return null;
            }
        }

        #endregion "TransferWise Account Statement"
    }
}