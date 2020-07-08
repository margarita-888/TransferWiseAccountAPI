using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseClient
{
    public class BalanceDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("profileId")]
        public int ProfileId { get; set; }

        [JsonProperty("recipientId")]
        public int RecipientId { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("modificationTime")]
        public DateTime ModificationTime { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("eligible")]
        public bool Eligible { get; set; }

        [JsonProperty("balances")]
        public Balance[] Balances { get; set; }
    }

    public class Balance
    {
        [JsonProperty("balanceType")]
        public string BalanceType { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public BalanceAmount Amount { get; set; }

        [JsonProperty("reservedAmount")]
        public ReservedAmount ReservedAmount { get; set; }

        [JsonProperty("bankDetails")]
        public BankDetails BankDetails { get; set; }
    }

    public class BalanceAmount
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class ReservedAmount
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class BankDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("bankCode")]
        public string BankCode { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("swift")]
        public string Swift { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("accountHolderName")]
        public string AccountHolderName { get; set; }

        [JsonProperty("bankAddress")]
        public BankAddress BankAddress { get; set; }
    }

    public class BankAddress
    {
        [JsonProperty("addressFirstLine")]
        public string AddressFirstLine { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("stateCode")]
        public string StateCode { get; set; }
    }
}