using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseWebhooks.DataModels
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
        public bool SignatureVerified { get; set; }

        public override string ToString()
        {
            return $"\n========================================\n" +
                $"Balance Deposit received:\n" +
                $"Balance Account Id:\t\t {BalanceAccountId}\n" +
                $"Balance Account Type:\t\t {BalanceAccountType}\n" +
                $"Profile Id:\t\t\t {ProfileId}\n" +
                $"Event Type:\t\t\t {EventType}\n" +
                $"Amount:\t\t\t\t {Amount}\n" +
                $"Currency:\t\t\t {Currency}\n" +
                $"Post Transaction Balance Amount: {PostTransactionBalanceAmount}\n" +
                $"OccurredAt:\t\t\t {OccurredAt}\n" +
                $"Sent At:\t\t\t {SentAt}\n" +
                $"Signature Verified:\t\t {SignatureVerified}\n" +
                $"========================================\n";
        }
    }
}