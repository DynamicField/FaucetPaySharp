using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace FaucetPaySharp
{
    public class FaucetPaySharpException : Exception
    {
        public int StatusCode { get; }
        public FaucetPaySharpException()
        {
        }

        public FaucetPaySharpException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public FaucetPaySharpException(string message) : base(message)
        {

        }
        public FaucetPaySharpException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public FaucetPaySharpException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public FaucetPaySharpException(string message, int statusCode, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
        protected FaucetPaySharpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public bool HasError(FaucetPayError error) => StatusCode == (int)error;
        public bool HasError(FaucetPayError error, params FaucetPayError[] errors) 
            => errors.Concat(new[] { error }).Any(e => StatusCode == (int)e);
    }

    public enum FaucetPayError
    {
        AccessDenied = 401,
        InvalidCurrency = 410,
        InvalidAddress = 456,
        InvalidPaymentAmount = 405,
        SendLimitReached = 450,
        InsufficientFunds = 402,
        NotFound = 404,
        InvalidApiKey = 403
    }
}