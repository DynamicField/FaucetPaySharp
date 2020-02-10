using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaucetPaySharp.Interceptors
{
    public static class SendInterceptorExtensions
    {
        public static async Task ThrowInterceptorErrors<T>(this IEnumerable<T> interceptors, Func<T, Task<InterceptorResult>> checker) where T : IInterceptor
        {
            foreach (var interceptor in interceptors)
            {
                var result = await checker(interceptor).ConfigureAwait(false);
                if (!result.IsSuccess) throw new InterceptorFailException(result, interceptor.GetType());
            }
        }
    }
}