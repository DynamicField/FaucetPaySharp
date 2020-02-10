using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaucetPaySharp.Models;

namespace FaucetPaySharp
{
    /// <summary>
    /// An API wrapper around the FaucetPay API.
    /// </summary>
    public interface IFaucetPayClient : IDisposable
    {
        /// <summary>
        /// Gets the balance of the faucet.
        /// </summary>
        /// <param name="currency">A currency used to select which balance to get.</param>
        /// <returns>The current balance, specified by <paramref name="currency"/>.</returns>
        Task<BalanceResponse> GetBalance(string currency);

        /// <summary>
        /// Gets all available currencies.
        /// </summary>
        /// <returns>The available currencies.</returns>
        Task<CurrenciesResponse> GetCurrencies();

        /// <summary>
        /// Checks if an address exists.
        /// </summary>
        /// <param name="address">The address to check.</param>
        /// <param name="currency">What currency the address is.</param>
        /// <returns>A <see cref="TryCheckAddressResult"/> instance which contains details about the address and its existence.</returns>
        Task<TryCheckAddressResult> TryCheckAddress(string address, string currency);

        /// <summary>
        /// Sends an amount of cryptocurrency to a specified address.
        /// </summary>
        /// <param name="satoshiAmount">How much cryptocurrency to send, in satoshis (10^8).</param>
        /// <param name="to">The address to send to.</param>
        /// <param name="currency">What currency to send.</param>
        /// <param name="isReferral">Whether or not this transaction is a referral commission.</param>
        /// <returns>The result, containing the remaining balance and payout information.</returns>
        Task<SendResponse> Send(long satoshiAmount, string to, string currency, bool isReferral = false);

        /// <summary>
        /// Gets the most recent payouts made.
        /// </summary>
        /// <param name="transactionCount">How much payouts to get, between 0 and 100.</param>
        /// <param name="currency">The cryptocurrency used in those payouts.</param>
        /// <returns>The payouts, ordered by the most recent first.</returns>
        Task<IEnumerable<Payout>> GetPayouts(int transactionCount, string currency);

        /// <summary>
        /// Does the same as <see cref="FaucetPayClient.TryCheckAddress"/>,
        /// but throws a <see cref="InvalidCryptocurrencyAddressException"/> exception when the address is invalid.
        /// </summary>
        /// <exception cref="InvalidCryptocurrencyAddressException">If the address is invalid.</exception>
        /// <returns>The address information.</returns>
        /// <inheritdoc cref="FaucetPayClient.TryCheckAddress"/>
        Task<CheckAddressResponse> CheckAddress(string address, string currency);
    }
}