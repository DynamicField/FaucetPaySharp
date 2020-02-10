using System.Threading.Tasks;
using FaucetPaySharp.Interceptors;
using Xunit;

namespace FaucetPaySharp.Tests
{
    public class SendInterceptorExtensionsTests
    {
        private const string Message = "message";

        private static Task ThrowWith(params CustomResultInterceptor[] interceptors)
        {
            return interceptors.ThrowInterceptorErrors(i => Task.FromResult(i.Result));
        }

        [Fact]
        public async Task FailResult_Throws()
        {
            var interceptor = CreateFailInterceptor();

            await Assert.ThrowsAsync<InterceptorFailException>(() => ThrowWith(interceptor));
        }
        [Fact]
        public async Task FailResult_ThrowsWithMessage()
        {
            var interceptor = CreateFailInterceptor();

            try
            {
                await ThrowWith(interceptor);
                Assert.True(false, "No exception has been thrown.");
            }
            catch (InterceptorFailException e)
            {
                Assert.Equal(Message, e.Message);
            }
        }
        [Fact]
        public async Task FailResult_ThrowsWithType()
        {
            var interceptor = CreateFailInterceptor();

            try
            {
                await ThrowWith(interceptor);
                Assert.True(false, "No exception has been thrown.");
            }
            catch (InterceptorFailException e)
            {
                Assert.Equal(typeof(CustomResultInterceptor), e.InterceptorType);
            }
        }
        [Fact]
        public async Task SuccessResult_DoesNotThrow()
        {
            var interceptor = CreateSuccessInterceptor();

            await ThrowWith(interceptor);
        }

        private static CustomResultInterceptor CreateFailInterceptor()
        {
            var interceptorResult = InterceptorResult.Fail(Message);
            var interceptor = new CustomResultInterceptor(interceptorResult);
            return interceptor;
        }

        private static CustomResultInterceptor CreateSuccessInterceptor()
        {
            var interceptorResult = InterceptorResult.Success();
            var interceptor = new CustomResultInterceptor(interceptorResult);
            return interceptor;
        }
    }
}