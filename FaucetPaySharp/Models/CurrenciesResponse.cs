using System.Collections.Generic;
using Newtonsoft.Json;

namespace FaucetPaySharp.Models
{
    public class CurrenciesResponse : FaucetPayResponse
    {
        [JsonProperty("currencies")]
        public IEnumerable<string> CurrenciesAbbreviations { get; set; }

        [JsonProperty("currencies_names")]
        public IEnumerable<Currency> Currencies { get; set; }
    }
}