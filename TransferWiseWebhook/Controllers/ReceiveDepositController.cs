﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TransferWiseWebhook.DataModels;
using TransferWiseCommon;

namespace TransferWiseWebhook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiveDepositController : ControllerBase
    {
        private const string BALANCE_CREDIT_EVENT = "balances#credit";
        private const string ALGORITHM_1 = "SHA1WITHRSA";

        private readonly ILogger _logger;

        public ReceiveDepositController(ILogger<Program> logger)
        {
            _logger = logger;
        }

        // This is used by Transferwise to inform us of new deposits into our account.
        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        public ActionResult ReceiveBalanceCredit([FromBody()] BalanceDepositDTO payload)
        {
            DateTime beginProcessingTimeUTC = DateTime.UtcNow;

            try
            {
                // Make sure a JSON object was provided in the body.
                if (payload == null)
                {
                    _logger.LogInformation("ReceiveDepositController::ReceivePayment. Transferwise balance credit. Body: <empty>.");
                    return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = "no payload in the Body." });
                }

                var jsonString = JsonConvert.SerializeObject(payload, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii,
                    Formatting = Newtonsoft.Json.Formatting.None,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });

                // Log the whole object.
                _logger.LogInformation("Transferwise balance credit. JSON object provided: [" + jsonString + "].");

                // We expect this payload was sent when the balances#credit event was raised at TransferWise
                if (payload.event_type != BALANCE_CREDIT_EVENT)
                {
                    _logger.LogInformation("ReceiveDepositController::ReceivePayment. Received TransferwiseInput object of wrong type " + payload.event_type + ".");
                    return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = "payload event type is NOT balances#credit." });
                }

                var totalProcessingTime = (DateTime.UtcNow - beginProcessingTimeUTC).TotalMilliseconds.ToString("###,###,##0");
                _logger.LogInformation($"Received Transferwise balance#credit payload. Total processing time is {totalProcessingTime} milliseconds.");
                //receivedSuccessfully = true;

                var headers = this.Request.Headers.ToList();

                var xSignatureHeader = headers.Find(h => h.Key.Equals(("X-Signature")));
                if (xSignatureHeader.Key == null || String.IsNullOrEmpty(xSignatureHeader.Value.FirstOrDefault()))
                {
                    _logger.LogInformation("ReceiveDepositController::SignatureVerified. Unable to find 'x-signature' header in TransferWise balances#credit webhook.");
                    return null;
                }

                var xSignature = xSignatureHeader.Value.FirstOrDefault();
                _logger.LogInformation($"\n'X-Signature' header: {xSignature}");

                var signatureVerified = SignatureHelper.VerifySignature(xSignature, jsonString, ALGORITHM_1);
                if (!signatureVerified)
                {
                    _logger.LogInformation("ReceiveDepositController::SignatureVerified. Unable to verify TransferWise signature for this webhook.");
                    return null;
                }

                var paymentReceived = new BalanceDeposit()
                {
                    BalanceAccountId = payload.data.resource.id,
                    BalanceAccountType = payload.data.resource.type,
                    ProfileId = payload.data.resource.profile_id,
                    EventType = payload.event_type,
                    TransactionType = payload.data.transaction_type,
                    Amount = payload.data.amount,
                    Currency = payload.data.currency,
                    PostTransactionBalanceAmount = payload.data.post_transaction_balance_amount,
                    OccurredAt = DateTime.Parse(payload.data.occurred_at.Remove(payload.data.occurred_at.Length - 1)),
                    SentAt = DateTime.Parse(payload.sent_at.Remove(payload.sent_at.Length - 1))
                };

                totalProcessingTime = (DateTime.UtcNow - beginProcessingTimeUTC).TotalMilliseconds.ToString("###,###,##0");
                _logger.LogInformation($"Verified signature. Total processing time is {totalProcessingTime} milliseconds.");

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReceiveDepositController::ReceivePayment. Exception, {ex.Message}. {ex.StackTrace}.");
                return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = "exception thrown." }); ;
            }
        }
    }
}