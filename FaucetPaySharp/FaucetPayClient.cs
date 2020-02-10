using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FaucetPaySharp.Http;
using FaucetPaySharp.Interceptors;
using FaucetPaySharp.Models;

namespace FaucetPaySharp
{
    /// <summary>
    /// An API wrapper around the FaucetPay API.
    /// </summary>
    public sealed class FaucetPayClient : IFaucetPayClient
    {
        /// <summary>
        /// The Bitcoin currency abbreviation.
        /// </summary>
        public const string Bitcoin = "BTC";

        /// <summary>
        /// The Dogecoin currency abbreviation.
        /// </summary>
        public const string Dogecoin = "DOGE";

        /// <summary>
        /// The Ethereum currency abbreviation.
        /// </summary>
        public const string Ethereum = "ETH";

        /// <summary>
        /// The Litecoin currency abbreviation.
        /// </summary>
        public const string Litecoin = "LTC";

        /// <summary>
        /// The Dash currency abbreviation.
        /// </summary>
        public const string Dash = "DASH";

        /// <summary>
        /// The Bitcoin Cash currency abbreviation.
        /// </summary>
        public const string BitcoinCash = "BCH";

        private readonly IRequester _requester;
        /// <summary>
        /// Create a new <see cref="FaucetPayClient"/> with a requester.
        /// </summary>
        /// <param name="requester">The requester to use.</param>
        public FaucetPayClient(IRequester requester)
        {
            _requester = requester;
        }

        /// <summary>
        /// Create a new <see cref="FaucetPayClient"/> with the default requester.
        /// </summary>
        /// <remarks>
        /// The default <see cref="HttpClient"/> should be disposed using <see cref="Dispose"/>,
        /// but a custom <see cref="HttpClient"/> is not disposed, even within <see cref="Dispose"/>.
        /// </remarks>
        /// <param name="config">The configuration used for the client</param>
        /// <param name="client">
        ///     A <see cref="HttpClient"/> used for requests, when set to null, a new instance will be used.
        /// </param>
        /// <returns>A <see cref="FaucetPayClient"/> instance.</returns>
        public static FaucetPayClient Create(ApiConfig config, HttpClient client = null)
        {
            var requester = HttpClientRequester.Create(config, client);
            return new FaucetPayClient(requester);
        }
        /// <summary>
        /// Disposes the client and its <see cref="IRequester"/>.
        /// </summary>
        /// <remarks>
        /// The requester will only be disposed if it implements <see cref="IDisposable"/>
        /// </remarks>
        public void Dispose()
        {
            if (_requester is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <inheritdoc />
        public async Task<BalanceResponse> GetBalance(string currency)
        {
            return await _requester.Post<BalanceResponse>("balance", new Dictionary<string, string>
            {
                ["currency"] = currency
            }).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CurrenciesResponse> GetCurrencies()
        {
            return await _requester.Post<CurrenciesResponse>("currencies").ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TryCheckAddressResult> TryCheckAddress(string address, string currency)
        {
            var result = await CheckAddressRequest(address, currency, true).ConfigureAwait(false);
            if (result.Status == 456) return new TryCheckAddressResult(address, currency, false, null);
            if (result.Status != 200)
            {
                RequesterBase.HandleError(result.Status, result.Message);
                return null;
            }
            return new TryCheckAddressResult(address, currency, true, result);
        }

        /// <inheritdoc />
        public async Task<SendResponse> Send(long satoshiAmount, string to, string currency, bool isReferral = false)
        {
            await _requester.Configuration.SendInterceptors.ThrowInterceptorErrors(
                i => i.CheckSendRequestAsync(satoshiAmount, to, currency, isReferral));
            return await _requester.Post<SendResponse>("send", new Dictionary<string, string>
            {
                ["amount"] = satoshiAmount.ToString(),
                ["to"] = to,
                ["currency"] = currency,
                ["referral"] = isReferral ? "true" : "false"
            }).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task<IEnumerable<Payout>> GetPayouts(int transactionCount, string currency)
        {
            if (transactionCount < 0 || transactionCount > 100) 
                throw new ArgumentOutOfRangeException($"The transaction count is out of range (0-100), got {transactionCount}.", nameof(transactionCount));

            return (await _requester.Post<PayoutsResponse>("payouts", new Dictionary<string, string>
            {
                ["currency"] = currency,
                ["count"] = transactionCount.ToString()
            }).ConfigureAwait(false)).Payouts;
        }

        /// <inheritdoc />
        public async Task<CheckAddressResponse> CheckAddress(string address, string currency)
        {
            try
            {
                return await CheckAddressRequest(address, currency).ConfigureAwait(false);
            }
            catch (FaucetPaySharpException e) when (e.HasError(FaucetPayError.InvalidAddress))
            {
                throw new InvalidCryptocurrencyAddressException(e.Message, e);
            }
        }

        private Task<CheckAddressResponse> CheckAddressRequest(string address, string currency, bool noThrow = false)
        {
            return _requester.Post<CheckAddressResponse>("checkaddress", new Dictionary<string, string>
            {
                ["address"] = address,
                ["currency"] = currency
            }, noThrow);
        }
    }
}