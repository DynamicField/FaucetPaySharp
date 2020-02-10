using System;
using System.Threading.Tasks;

namespace FaucetPaySharp.Interceptors
{
    /// <summary>
    /// An interceptor to set limits for send requests amounts, and fail when the limit is reached.
    /// </summary>
    public class LimitSendInterceptor : ISendInterceptor
    {
        /// <summary>
        /// The currency to limit.
        /// </summary>
        public string Currency { get; }
        /// <summary>
        /// The limit to apply for each transaction, in satoshi
        /// </summary>
        public long SatoshiLimit { get; }

        /// <summary>
        /// Creates a <see cref="LimitSendInterceptor"/>
        /// </summary>
        /// <param name="currency">The currency to limit.</param>
        /// <param name="satoshiLimit">The actual limit, in satoshis.</param>
        public LimitSendInterceptor(string currency, long satoshiLimit)
        {
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            SatoshiLimit = satoshiLimit;
        }
        public Task<InterceptorResult> CheckSendRequestAsync(long satoshiAmount, string to, string currency,
            bool isReferral)
        {
            if (!Currency.Equals(currency, StringComparison.OrdinalIgnoreCase)) return Task.FromResult(InterceptorResult.Success());
            return Task.FromResult(InterceptorResult.IfFailure(satoshiAmount > SatoshiLimit,
                                        () => $"The send value ({satoshiAmount} {currency} satoshi) is higher than limit ({SatoshiLimit} {currency} satoshi)."));
        }
    }
}