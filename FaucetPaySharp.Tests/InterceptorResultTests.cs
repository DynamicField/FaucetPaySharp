using System;
using FaucetPaySharp.Interceptors;
using Xunit;

namespace FaucetPaySharp.Tests
{
    public class InterceptorResultTests
    {
        private const string Message = "message";

        [Fact]
        public void Fail_HasSuccessFalse()
        {
            var result = InterceptorResult.Fail(Message);

            Assert.False(result.IsSuccess);
        }
        [Fact]
        public void Fail_HasMessage()
        {
            var result = InterceptorResult.Fail(Message);

            Assert.Equal(Message, result.Message);
        }

        [Fact]
        public void Success_HasSuccessTrue()
        {
            var result = InterceptorResult.Success();

            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void Success_HasNoMessage()
        {
            var result = InterceptorResult.Success();

            Assert.Null(result.Message);
        }

        [Fact]
        public void IfFailure_StringSuccess_GivesSuccess()
        {
            var result = InterceptorResult.IfFailure(false, Message);

            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void IfFailure_StringFail_GivesFail()
        {
            var result = InterceptorResult.IfFailure(true, Message);

            Assert.False(result.IsSuccess);
        }
        [Fact]
        public void IfFailure_StringFail_GivesMessage()
        {
            var result = InterceptorResult.IfFailure(true, Message);

            Assert.Equal(Message, result.Message);
        }

        [Fact]
        public void IfFailure_FuncStringSuccess_GivesSuccess()
        {
            var result = InterceptorResult.IfFailure(false, () => Message);

            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void IfFailure_FuncStringSuccess_DoesNotCallFunc()
        {
            var called = false;

            InterceptorResult.IfFailure(false, () =>
            {
                called = true;
                return Message;
            });

            Assert.False(called);
        }
        [Fact]
        public void IfFailure_FuncStringFail_GivesFail()
        {
            var result = InterceptorResult.IfFailure(true, () => Message);

            Assert.False(result.IsSuccess);
        }
        [Fact]
        public void IfFailure_FuncStringFail_GivesMessage()
        {
            var result = InterceptorResult.IfFailure(true, () => Message);

            Assert.Equal(Message, result.Message);
        }

    }
}