using System;
using System.Net.Http;
using FaucetPaySharp.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace FaucetPaySharp.AspNetCore
{
    public static class FaucetPaySharpExtensions
    {
        private class RequesterTypedFactory<T> : ITypedHttpClientFactory<T> where T : IRequester
        {
            private readonly IServiceProvider _provider;
            private readonly IOptionsMonitor<ApiConfig> _configOptions;

            public RequesterTypedFactory(IServiceProvider provider, IOptionsMonitor<ApiConfig> configOptions)
            {
                _provider = provider;
                _configOptions = configOptions;
            }

            public T CreateClient(HttpClient httpClient)
            {               
                var options = _configOptions.CurrentValue;
                HttpClientRequester.ConfigureClient(httpClient, options);
                return ActivatorUtilities.CreateInstance<T>(_provider, httpClient, options);
            }
        }
        public static IServiceCollection AddFaucetPaySharp(this IServiceCollection services, Action<ApiConfig> configure = null)
        {
            if (configure != null)
                services.Configure(configure);
            services.AddSingleton(typeof(ITypedHttpClientFactory<HttpClientRequester>), typeof(RequesterTypedFactory<HttpClientRequester>));
            services.AddHttpClient<IRequester, HttpClientRequester>();
            services.AddTransient<FaucetPayClient>();
            return services;
        }
    }
}
