using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TransferWiseWebhooks.DataModels;

namespace TransferWiseWebhooks.Interfaces
{
    public interface ITransferWiseEventProcessor
    {
        public Task<IActionResult> ProcessBalanceCreditEventAsync(BalanceDepositDTO payload, string signature);
    }
}