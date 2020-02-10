using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FaucetPaySharp.Models
{
    internal class PayoutsResponse : FaucetPayResponse
    {
        [JsonProperty("rewards")]
        public IEnumerable<Payout> Payouts { get; set; }
    }

    public class Payout
    {
        public string To { get; set; }

        [JsonProperty("amount")]
        public long SatoshiAmount { get; set; }

        public DateTime Date { get; set; }
    }
}