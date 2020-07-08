using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseClient
{
    public class StatementDTO
    {
        [JsonProperty("accountHolder")]
        public AccountHolder AccountHolder { get; set; }

        [JsonProperty("issuer")]
        public Issuer Issuer { get; set; }

        [JsonProperty("bankDetails")]
        public BankDetails BankDetails { get; set; }

        [JsonProperty("transactions")]
        public Transaction[] Transactions { get; set; }

        [JsonProperty("endOfStatementBalance")]
        public EndOfStatementBalance EndOfStatementBalance { get; set; }

        [JsonProperty("query")]
        public Query Query { get; set; }
    }

    public class AccountHolder
    {
        [JsonProperty("type")]
        public string Type { get; set; }     // PERSONAL or BUSINESS

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }

    public class Address
    {
        [JsonProperty("addressFirstLine")]
        public string AddressFirstLine { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("stateCode")]
        public string StateCode { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }
    }

    public class Issuer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("firstLine")]
        public string FirstLine { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("stateCode")]
        public string StateCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("type")]
        public string Type { get; set; }  // DEBIT or CREDIT

        [JsonProperty("date")]
        public DateTime TransactionDate { get; set; }  // Transaction Date

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("totalFees")]
        public TotalFees TotalFees { get; set; }

        [JsonProperty("details")]
        public Details TransactionDetails { get; set; }

        [JsonProperty("exchangeDetails")]
        public ExchangeDetails ExchangeDetails { get; set; }

        [JsonProperty("runningBalance")]
        public RunningBalance RunningBalance { get; set; }

        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }
    }

    public class Amount
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class TotalFees
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Details
    {
        [JsonProperty("type")]
        public string Type { get; set; }   // CARD, CONVERSION, DEPOSIT, TRANSFER, MONEY_ADDED, INCOMING_CROSS_BALANCE, OUTGOING_CROSS_BALANCE, DIRECT_DEBIT

        [JsonProperty("description")]
        public string Description { get; set; }   // Human readable explanation about the transaction

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("merchant")]
        public Merchant Merchant { get; set; }

        [JsonProperty("senderName")]
        public string SenderName { get; set; }

        [JsonProperty("senderAccount")]
        public string SenderAccount { get; set; }

        [JsonProperty("paymentReference")]
        public string PaymentReference { get; set; }

        [JsonProperty("sourceAmount")]
        public SourceAmount SourceAmount { get; set; }

        [JsonProperty("targetAmount")]
        public TargetAmount TargetAmount { get; set; }

        [JsonProperty("fee")]
        public Fee Fee { get; set; }

        [JsonProperty("rate")]
        public Decimal? Rate { get; set; }
    }

    public class Merchant
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("firstLine")]
        public string FirstLine { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }

    public class SourceAmount
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class TargetAmount
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Fee
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class ExchangeDetails
    {
        [JsonProperty("forAmount")]
        public ForAmount ForAmount { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }
    }

    public class ForAmount
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class RunningBalance
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class EndOfStatementBalance
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Query
    {
        [JsonProperty("intervalStart")]
        public DateTime IntervalStart { get; set; }   // Zulu time

        [JsonProperty("intervalEnd")]
        public DateTime IntervalEnd { get; set; }   // Zulu time

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("accountId")]
        public int AccountId { get; set; }
    }
}