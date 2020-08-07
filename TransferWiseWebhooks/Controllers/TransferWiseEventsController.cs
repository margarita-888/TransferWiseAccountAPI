using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;
using TransferWiseWebhooks.DataModels;
using TransferWiseWebhooks;
using TransferWiseWebhooks.Interfaces;

namespace TransferWiseWebhooks.Controllers
{
    [Route("api/transferwiseevents")]
    [ApiController]
    public class TransferWiseEventsController : ControllerBase
    {
        private readonly ITransferWiseEventProcessor _transferWiseEventProcessor;
        private readonly ILogger<Program> _logger;

        public TransferWiseEventsController(ITransferWiseEventProcessor transferWiseEventProcessor, ILogger<Program> logger)
        {
            _transferWiseEventProcessor = transferWiseEventProcessor ?? throw new ArgumentNullException(nameof(transferWiseEventProcessor));
            _logger = logger;
        }

        [HttpPost]
        [Route("balancecredit")]
        [Produces("application/json")]
        public async Task<IActionResult> ProcessBalanceCreditEventAsync([FromBody()] BalanceDepositDTO payload)
        {
            string signature = string.Empty;
            var headers = this.Request.Headers;
            if (headers.ContainsKey("X-Signature-SHA256"))
            {
                var signatureHeader = headers.ToList().Find(h => h.Key.Equals("X-Signature-SHA256"));
                signature = signatureHeader.Value.FirstOrDefault();
            }
            try
            {
                return await _transferWiseEventProcessor.ProcessBalanceCreditEventAsync(payload, signature);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TransferWiseEventsController::ProcessBalanceCreditEventAsync. Exception, {ex.Message}. {ex.StackTrace}.");
                return new BadRequestObjectResult(new { StatusCode = StatusCodes.Status400BadRequest, message = "exception thrown." }); ;
            }
        }
    }
}