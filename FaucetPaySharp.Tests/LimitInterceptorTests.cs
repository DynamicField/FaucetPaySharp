using System.Threading.Tasks;
using FaucetPaySharp.Interceptors;
using Xunit;

namespace FaucetPaySharp.Tests
{
    public class LimitInterceptorTests
    {
        private const string Currency = FaucetPayClient.Bitcoin;
        private const string Address = "some address";
        private const int Limit = 500;

        [Fact]
        public async Task LimitReached_Fails()
        {
            const int transactionValue = 600;

            var interceptor = new LimitSendInterceptor(Currency, Limit);
            var result = await interceptor.CheckSendRequestAsync(transactionValue, Address, Currency, false);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task LimitNotReached_Success()
        {
            const int transactionValue = 400;

            var interceptor = new LimitSendInterceptor(Currency, Limit);
            var result = await interceptor.CheckSendRequestAsync(transactionValue, Address, Currency, false);

            Assert.True(result.IsSuccess);
        }
    }
}