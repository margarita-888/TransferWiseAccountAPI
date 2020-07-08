using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseAccountAPI.DataModels
{
    public class BalanceDeposit
    {
        public int BalanceAccountId { get; set; } // ID of the balance account
        public string BalanceAccountType { get; set; } // Balance account resource type (always balance-account)
        public int ProfileId { get; set; }
        public string EventType { get; set; } // balances#credit
        public string TransactionType { get; set; } // Always credit
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double PostTransactionBalanceAmount { get; set; }
        public DateTime OccurredAt { get; set; } // When the balance credit occurred
        public DateTime SentAt { get; set; } // When the event notification was sent from TransferWise system
    }
}