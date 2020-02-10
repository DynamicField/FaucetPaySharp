using System;

namespace FaucetPaySharp.Interceptors
{
    public class InterceptorResult
    { 
        public bool IsSuccess { get; }
        public string Message { get; }

        private InterceptorResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public static InterceptorResult Success()
        {
            return new InterceptorResult(true, null);
        }

        public static InterceptorResult Fail(string message)
        {
            return new InterceptorResult(false, message);
        }

        public static InterceptorResult IfFailure(bool failed, string message)
        {
            if (failed) return Fail(message);
            return Success();
        }

        public static InterceptorResult IfFailure(bool failed, Func<string> message)
        {
            if (failed) return Fail(message());
            return Success();
        }
    }
}