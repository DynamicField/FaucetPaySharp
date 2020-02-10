using System;
using System.Runtime.Serialization;

namespace FaucetPaySharp.Interceptors
{
    public class InterceptorFailException : Exception
    {
        public Type InterceptorType { get; }
        public InterceptorResult Result { get; }

        public InterceptorFailException()
        {
        }

        protected InterceptorFailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InterceptorFailException(string message) : base(message)
        {
        }
        public InterceptorFailException(string message, InterceptorResult result, Type interceptorType = null) : base(message)
        {
            InterceptorType = interceptorType;
            Result = result;
        }
        public InterceptorFailException(InterceptorResult result, Type interceptorType = null) : base(result.Message)
        {
            InterceptorType = interceptorType;
            Result = result;
        }
        public InterceptorFailException(string message, Exception innerException, Type interceptorType = null) : base(message, innerException)
        {
            InterceptorType = interceptorType;
        }
        public InterceptorFailException(string message, InterceptorResult result, Exception innerException,
            Type interceptorType = null) : base(message, innerException)
        {
            InterceptorType = interceptorType;
            Result = result;
        }
    }
}