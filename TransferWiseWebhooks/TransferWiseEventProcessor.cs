using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TransferWiseWebhooks.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TransferWiseWebhooks.DataModels;
using Newtonsoft.Json;
using System.Diagnostics;
using TransferWiseCommon;

namespace TransferWiseWebhooks
{
    public class TransferWiseEventProcessor : ITransferWiseEventProcessor
    {
        private readonly ILogger<Program> _logger;

        public TransferWiseEventProcessor(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> ProcessBalanceCreditEventAsync(BalanceDepositDTO payload, string signature)
        {
            string BALANCE_CREDIT = "balances#credit";

            // Make sure a JSON object was provided in the body.
            if (payload == null)
            {
                _logger.LogInformation("TransferWiseEventProcessor::ProcessBalanceCreditEventAsync. Error. Request Body is empty.");
                return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = "Request Body is empty." });
            }

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogInformation("TransferWiseEventProcessor::ProcessBalanceCreditEventAsync. Error. X-Signature-SHA256 Not found in the request.");
                return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = " X-Signature-SHA256 Not found in the request." });
            }

            // We expect this payload was sent when the balances#credit event was raised at TransferWise
            if (payload.EventType != BALANCE_CREDIT)
            {
                _logger.LogInformation($"TransferWiseEventProcessor::ProcessBalanceCreditEventAsync. Error. Payload event type was NOT {BALANCE_CREDIT}.");
                return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = $"Payload event type was NOT {BALANCE_CREDIT}." });
            }

            var balanceDeposit = await ProcessBalanceCreditAsync(signature, payload).ConfigureAwait(false);
            _logger.LogInformation($"TransferWiseEventProcessor::ProcessBalanceCreditEventAsync. {balanceDeposit}.");

            if (balanceDeposit.SignatureVerified)
                return new OkObjectResult(balanceDeposit);
            else
            {
                _logger.LogInformation($"TransferWiseEventProcessor::ProcessBalanceCreditEventAsync. Error. Invalid signature.");
                return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = "Invalid signature." });
            }
        }

        public async Task<BalanceDeposit> ProcessBalanceCreditAsync(string signature, BalanceDepositDTO payload)
        {
            return await Task.Run(() => ProcessBalanceCredit(signature, payload));
        }

        private BalanceDeposit ProcessBalanceCredit(string signature, BalanceDepositDTO payload)
        {
            try
            {
                // Verify that the event was actually sent by TransferWise by checking the "X-Signature-SHA256" header value
                var signatureVerified = VerifyTransferWiseSignature(signature, payload);

                return payload.DTOtoBalanceDeposit(signatureVerified);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TransferWiseEventProcessor::ProcessBalanceCredit. Exception, {ex.Message}. {ex.StackTrace}.");
                return null;
            }
        }

        private bool VerifyTransferWiseSignature(string signature, BalanceDepositDTO payload)
        {
            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogError($"TransferWiseEventProcessor::VerifyTransferWiseSignature. Error. X-Signature-SHA256 must be provided. Unable to continue.");
                return false;
            }

            var json = JsonConvert.SerializeObject(payload);

            // Log the whole object.
            _logger.LogInformation($"TransferWiseEventProcessor::VerifyTransferWiseSignature.\nJSON string:\n[{json}].");

            var signatureVerified = SignatureHelper.VerifySignature(signature, json);
            if (!signatureVerified)
            {
                _logger.LogInformation("TransferWiseEventProcessor::VerifyTransferWiseSignature. Error. Attempt to verify signature for Balance Credit Event returned false.");
                return false;
            }

            _logger.LogInformation("TransferWiseEventProcessor::VerifyTransferWiseSignature. Signature for Balance Credit Event verified successfully.");
            return true;
        }
    }
}