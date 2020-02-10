using System;

namespace FaucetPaySharp
{
    public static class CryptocurrencyExtensions
    {
        private const string InaccuracyMessage =
            "Using double to store coin values is not recommended. This can lead to eventual inaccuracies. Please use a decimal value instead.";

        public static long ToSatoshi(this decimal coinValue) => (long)(coinValue * 100000000m);

        [Obsolete(InaccuracyMessage)]
        public static long ToSatoshi(this double coinValue) => (long)(coinValue * 100000000);

        public static decimal ToCoinValue(this long satoshiValue) => satoshiValue / 100000000m;
    }
}