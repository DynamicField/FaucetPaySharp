using System.Threading.Tasks;

namespace FaucetPaySharp.Interceptors
{
    /// <summary>
    /// The base interface for all interceptors.
    /// </summary>
    public interface IInterceptor { }
    /// <summary>
    /// An interceptor to monitor send requests.
    /// </summary>
    public interface ISendInterceptor : IInterceptor
    {
        /// <summary>
        /// Checks the send request.
        /// </summary>
        /// <returns>The <see cref="InterceptorResult"/></returns>
        /// <inheritdoc cref="IFaucetPayClient.Send"/>
        Task<InterceptorResult> CheckSendRequestAsync(long satoshiAmount, string to, string currency, bool isReferral);
    }
}