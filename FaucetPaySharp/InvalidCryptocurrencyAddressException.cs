using System;

namespace FaucetPaySharp
{
    public class InvalidCryptocurrencyAddressException : FaucetPaySharpException
    {
        private const int ErrorCode = (int) FaucetPayError.InvalidAddress;
        public InvalidCryptocurrencyAddressException() : base(ErrorCode)
        {
            
        }
        public InvalidCryptocurrencyAddressException(string message) : base(message, ErrorCode)
        {
        }

        public InvalidCryptocurrencyAddressException(string message, Exception innerException) : base(message, ErrorCode, innerException)
        {
        }
    }
}