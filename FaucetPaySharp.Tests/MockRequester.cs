using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaucetPaySharp.Http;

namespace FaucetPaySharp.Tests
{
    internal class MockRequester : RequesterBase
    {
        // public delegate void Checker(Dictionary<string, string> parameters);

        private readonly IEnumerable<(string resource, string json)> _data;
        //private readonly List<(string resource, Checker checker)> _parameterCheckers
        //    = new List<(string resource, Checker checker)>();
        private static ApiConfig WithApiKey(ApiConfig config)
        {
            config ??= new ApiConfig();
            config.ApiKey ??= "!";
            return config;
        }
        public MockRequester(ApiConfig config, string resource, string json) : this(WithApiKey(config), (resource, json)) { }

        public MockRequester(string resource, string json) : this((resource, json)) { }

        public MockRequester(params (string resource, string json)[] data) : this(null, data) { }

        public MockRequester(ApiConfig config, params (string resource, string json)[] data) 
            : base(WithApiKey(config) ?? new ApiConfig { ApiKey = "!" })
        {
            _data = data;
        }

        public override Task<T> Post<T>(string resource, Dictionary<string, string> parameters = null, bool noThrow = false)
        {
            //foreach (var (_, checker) in _parameterCheckers.Where(p => p.resource == resource))
            //{
            //    checker(parameters);
            //}
            foreach (var (r, json) in _data)
            {
                if (r != resource) continue;
                var result = Deserialize<T>(json);
                HandleError(result.Status, result.Message, noThrow: noThrow);
                return Task.FromResult(result);
            }

            return Task.FromResult<T>(default);
        }

        //public MockRequester WithCheck(Checker checker)
        //{
        //    _parameterCheckers.Add((_data.First().resource, checker));
        //    return this;
        //}

        //public MockRequester WithCheck(string resource, Checker checker)
        //{
        //    _parameterCheckers.Add((resource, checker));
        //    return this;
        //}
        public FaucetPayClient CreateClient() => new FaucetPayClient(this);
    }
}