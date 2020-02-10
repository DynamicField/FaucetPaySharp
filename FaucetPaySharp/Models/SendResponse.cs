using Newtonsoft.Json;

namespace FaucetPaySharp.Models
{
    public class SendResponse : FaucetPayResponse
    {
        public float? RateLimitRemaining { get; set; }
        public string Currency { get; set; }

        [JsonProperty("balance")]
        public long RemainingSatoshiBalance { get; set; }
        [JsonProperty("balance_bitcoin")]
        public decimal RemainingActualBalance { get; set; }

        public int PayoutId { get; set; }
        public string PayoutUserHash { get; set; }
    }
}