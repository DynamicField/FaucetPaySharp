using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FaucetPaySharp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FaucetPaySharp.Http
{
    public abstract class RequesterBase : IRequester
    {
        public RequesterBase(ApiConfig configuration)
        {
            Configuration = configuration;
            configuration.Check();
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        protected virtual T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, Settings);
        protected virtual string Serialize(object obj) => JsonConvert.SerializeObject(obj, Settings);

        protected IEnumerable<KeyValuePair<string, string>> CreateParameterPairs(IEnumerable<KeyValuePair<string, string>> source = null)
        {
            yield return new KeyValuePair<string, string>("api_key", Configuration.ApiKey);
            if (source == null) yield break;
            foreach (var item in source)
            {
                yield return item;
            }
        }

        public ApiConfig Configuration { get; }

        public static void HandleError(int statusCode, string message, Exception innerException = null, bool noThrow = false)
        {
            if (noThrow) return;
            if (statusCode >= 200 && statusCode <= 299) return; // Success!
            throw new FaucetPaySharpException(message, statusCode, innerException);
        }

        public abstract Task<T> Post<T>(string resource, Dictionary<string, string> parameters = null,
            bool noThrow = false) where T : FaucetPayResponse;
    }
}