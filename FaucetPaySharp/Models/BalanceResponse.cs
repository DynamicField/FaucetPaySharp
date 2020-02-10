using System.Reflection;
using Newtonsoft.Json;

namespace FaucetPaySharp.Models
{
    public class BalanceResponse : FaucetPayResponse
    {
        public string Currency { get; set; }

        [JsonProperty("balance")]
        public long SatoshiBalance { get; set; }

        [JsonProperty("balance_bitcoin")]
        public decimal ActualBalance { get; set; }

        public override string ToString()
        {
            return $"Faucet {Currency} balance: {ActualBalance}{Currency}";
        }
    }
}