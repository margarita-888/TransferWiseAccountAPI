using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransferWiseWebhooks.DataModels
{
    public class BalanceDepositDTO
    {
        [JsonProperty("data")]
        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonProperty("subscription_id")]
        [JsonPropertyName("subscription_id")]
        public string SubscriptionId { get; set; }

        [JsonProperty("event_type")]
        [JsonPropertyName("event_type")]
        public string EventType { get; set; }

        [JsonProperty("schema_version")]
        [JsonPropertyName("schema_version")]
        public string SchemaVersion { get; set; }

        [JsonProperty("sent_at")]
        [JsonPropertyName("sent_at")]
        public string SentAt { get; set; }

        public BalanceDeposit DTOtoBalanceDeposit(bool verified) => new BalanceDeposit
        {
            BalanceAccountId = string.IsNullOrEmpty(Data.Resource.Id.ToString()) ? 0 : Data.Resource.Id,
            BalanceAccountType = string.IsNullOrEmpty(Data.Resource.Type) ? null : Data.Resource.Type,
            ProfileId = string.IsNullOrEmpty(Data.Resource.ProfileId.ToString()) ? 0 : Data.Resource.ProfileId,
            EventType = string.IsNullOrEmpty(EventType) ? null : EventType,
            TransactionType = string.IsNullOrEmpty(Data.TransactionType) ? null : Data.TransactionType,
            Amount = string.IsNullOrEmpty(Data.Amount.ToString()) ? 0.0 : Data.Amount,
            Currency = string.IsNullOrEmpty(Data.Currency) ? null : Data.Currency,
            PostTransactionBalanceAmount = string.IsNullOrEmpty(Data.PostTransactionBalanceAmount.ToString()) ? 0.0 : Data.PostTransactionBalanceAmount,
            OccurredAt = string.IsNullOrEmpty(Data.OccurredAt) ? DateTime.MinValue : DateTime.Parse(Data.OccurredAt.Remove(Data.OccurredAt.Length - 1)),
            SentAt = string.IsNullOrEmpty(SentAt) ? DateTime.MinValue : DateTime.Parse(SentAt.Remove(SentAt.Length - 1)),
            SignatureVerified = verified
        };
    }

    public class Data
    {
        [JsonProperty("resource")]
        [JsonPropertyName("resource")]
        public Resource Resource { get; set; }

        [JsonProperty("amount")]
        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonProperty("currency")]
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonProperty("transaction_type")]
        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }

        [JsonProperty("post_transaction_balance_amount")]
        [JsonPropertyName("post_transaction_balance_amount")]
        public double PostTransactionBalanceAmount { get; set; }

        [JsonProperty("occurred_at")]
        [JsonPropertyName("occurred_at")]
        public string OccurredAt { get; set; }
    }

    public class Resource
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonProperty("profile_id")]
        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }
    }
}