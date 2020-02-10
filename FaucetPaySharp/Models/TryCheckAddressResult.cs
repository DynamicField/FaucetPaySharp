namespace FaucetPaySharp.Models
{
    /// <summary>
    /// The result of a <see cref="TryCheckAddress"/> request.
    /// </summary>
    public sealed class TryCheckAddressResult
    {
        internal TryCheckAddressResult(string address, string currency, bool isValidAddress, CheckAddressResponse response)
        {
            Address = address;
            Currency = currency;
            IsValidAddress = isValidAddress;
            Response = response;
        }

        public string Address { get; }
        public string Currency { get; set; }

        /// <summary>
        /// Whether or not the address is a valid address.
        /// </summary>
        public bool IsValidAddress { get; }
        public CheckAddressResponse Response { get; }
    }
}