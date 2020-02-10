using System;
using System.Collections.Generic;
using FaucetPaySharp.Interceptors;

namespace FaucetPaySharp
{
    /// <summary>
    /// The configuration for <see cref="IFaucetPayClient"/>.
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// The API key to use.
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// The base URL to use. defaults to the faucetpay api.
        /// </summary>
        public string BaseUrl { get; set; } = "https://faucetpay.io/api/v1/";

        /// <summary>
        /// The <see cref="ISendInterceptor"/> to use, when doing requests using <see cref="IFaucetPayClient.Send"/>.
        /// </summary>
        public IEnumerable<ISendInterceptor> SendInterceptors { get; set; } = Array.Empty<ISendInterceptor>();

        public void Check()
        {
            if (string.IsNullOrEmpty(ApiKey)) throw new ArgumentException("The API key is missing.", nameof(ApiKey));
        }
    }
}