using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseAccountAPI.DataModels
{
    public class BalanceDepositDTO
    {
        public Data data { get; set; }
        public string subscription_id { get; set; }
        public string event_type { get; set; }
        public string schema_version { get; set; }
        public string sent_at { get; set; }
    }

    public class Resource
    {
        public int id { get; set; }
        public string type { get; set; }
        public int profile_id { get; set; }
    }

    public class Data
    {
        public Resource resource { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string transaction_type { get; set; }
        public double post_transaction_balance_amount { get; set; }
        public string occurred_at { get; set; }
    }
}