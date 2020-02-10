using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FaucetPaySharp.Models;

namespace FaucetPaySharp.Http
{
    /// <summary>
    /// The default requester, using <see cref="HttpClient"/>.
    /// </summary>
    public class HttpClientRequester : RequesterBase, IDisposable
    {
        private readonly HttpClient _client;
        private bool _disposeClient;
        public HttpClientRequester(ApiConfig configuration,
                                   HttpClient client) : base(configuration)
        {
            _client = client;
        }
        /// <summary>
        /// Configure a <see cref="HttpClient"/> instance using a specified configuration.
        /// </summary>
        /// <param name="client">The client to configure.</param>
        /// <param name="configuration">The configuration to apply.</param>
        public static void ConfigureClient(HttpClient client, ApiConfig configuration)
        {
            client.BaseAddress = new Uri(configuration.BaseUrl);
        }

        protected internal virtual Task<HttpResponseMessage> CreateRequest(string resource, HttpMethod method, Dictionary<string, string> parameters)
        {
            var content = new FormUrlEncodedContent(CreateParameterPairs(parameters));

            return _client.SendAsync(new HttpRequestMessage(method, new Uri(resource, UriKind.Relative))
            {
                Content = content
            });
        }

        public override async Task<T> Post<T>(string resource, Dictionary<string, string> parameters = null,
            bool noThrow = false)
        {
            var result = await CreateRequest(resource, HttpMethod.Post, parameters).ConfigureAwait(false);

            var response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            var deserialized = Deserialize<T>(response);

            HandleError(deserialized.Status, deserialized.Message, noThrow: noThrow);

            return deserialized;
        }

        public static HttpClientRequester Create(ApiConfig config, HttpClient client = null, bool disposeClient = false)
        {
            if (client == null) disposeClient = true;
            client = client ?? new HttpClient();
            ConfigureClient(client, config);
            return new HttpClientRequester(config, client) { _disposeClient = disposeClient };
        }

        public void Dispose()
        {
            if (_disposeClient)
                _client.Dispose();
        }
    }
}