using System.Threading.Tasks;
using FaucetPaySharp.Interceptors;

namespace FaucetPaySharp.Tests
{
    internal class CustomResultInterceptor : IInterceptor
    {
        public InterceptorResult Result { get; }

        public CustomResultInterceptor(InterceptorResult result)
        {
            Result = result;
        }
    }
    internal class CustomResultSendInterceptor : CustomResultInterceptor, ISendInterceptor
    {
        public CustomResultSendInterceptor(InterceptorResult result) : base(result)
        {
        }

        public Task<InterceptorResult> CheckSendRequestAsync(long satoshiAmount, string to, string currency, bool isReferral)
        {
            return Task.FromResult(Result);
        }
    }
}