using Newtonsoft.Json;

namespace FaucetPaySharp.Models
{
    public abstract class FaucetPayResponse
    {
        [JsonProperty]
        internal int Status { get; set; }        

        [JsonProperty]
        internal string Message { get; set; }
    }
}