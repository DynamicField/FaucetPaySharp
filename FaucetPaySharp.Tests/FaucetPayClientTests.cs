using System;
using System.Linq;
using System.Threading.Tasks;
using ExpectedObjects;
using FaucetPaySharp.Interceptors;
using FaucetPaySharp.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FaucetPaySharp.Tests
{
    public class FaucetPayClientTests
    {
        [Fact]
        public async Task GetBalance_OK_HasCorrectData()
        {
            const string currency = FaucetPayClient.Bitcoin;
            const string json = @"{
    ""status"": 200,
    ""message"": ""OK"",
    ""currency"": ""BTC"",
    ""balance"": ""4321"",
    ""balance_bitcoin"": ""0.00004321""
}";
            var expectedResult = new BalanceResponse
            {
                Currency = FaucetPayClient.Bitcoin,
                ActualBalance = 0.00004321m,
                SatoshiBalance = 4321
            }.ToExpectedObject();

            var client = new MockRequester("balance", json).CreateClient();
            var actualResult = await client.GetBalance(currency);

            expectedResult.ShouldEqual(actualResult);
        }

        [Fact]
        public async Task GetCurrencies_OK_HasCorrectData()
        {
            const string json = @"{
    ""status"": 200,
    ""message"": ""OK"",
    ""currencies"": [
        ""BTC"",
        ""ETH""
    ],
    ""currencies_names"": [
        {
            ""name"": ""Bitcoin"",
            ""acronym"": ""BTC""
        },
        {
            ""name"": ""Ethereum"",
            ""acronym"": ""ETH""
        }
    ]
}";
            var expectedResult = new CurrenciesResponse
            {
                Currencies = new[]
                {
                    new Currency
                    {
                        Name = "Bitcoin",
                        Acronym = "BTC"
                    },
                    new Currency
                    {
                        Name = "Ethereum",
                        Acronym = "ETH"
                    }
                }.ToList(),
                CurrenciesAbbreviations = new[]
                {
                    "BTC", "ETH"
                }.ToList()
            }.ToExpectedObject();

            var client = new MockRequester("currencies", json).CreateClient();
            var actualResult = await client.GetCurrencies();

            expectedResult.ShouldEqual(actualResult);
        }

        [Fact]
        public async Task Send_OK_HasCorrectData()
        {
            const string currency = FaucetPayClient.Bitcoin;
            const string json = @"{
    ""status"": 200,
    ""message"": ""OK"",
    ""rate_limit_remaining"": 1,
    ""currency"": ""BTC"",
    ""balance"": ""4321"",
    ""balance_bitcoin"": ""0.00004321"",
    ""payout_id"": 1234,
    ""payout_user_hash"": ""hash""
}";
            var expectedResult = new SendResponse
            {
                RateLimitRemaining = 1,
                Currency = currency,
                RemainingSatoshiBalance = 4321,
                RemainingActualBalance = 0.00004321m,
                PayoutId = 1234,
                PayoutUserHash = "hash"
            }.ToExpectedObject();

            var client = new MockRequester("send", json).CreateClient();
            var actualResult = await client.Send(6789, "address", currency);

            expectedResult.ShouldMatch(actualResult);
        }

        [Fact]
        public async Task Send_WithFailInterceptor_ThrowsInterceptorFailException()
        {
            var client = new MockRequester(new ApiConfig
            {
                SendInterceptors = new [] { new CustomResultSendInterceptor(InterceptorResult.Fail("fail!")) }
            },"send", null).CreateClient();

            await Assert.ThrowsAsync<InterceptorFailException>(() =>  client.Send(6789, "address", FaucetPayClient.Bitcoin));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetPayouts_OK_HasCorrectData(int count)
        {
            var payout = new Payout
            {
                To = "address",
                SatoshiAmount = 100,
                Date = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };
            var expectedResult = Enumerable.Range(1, count).Select(x => payout).ToList().ToExpectedObject();

            const string payoutJson = @"{
    ""to"": ""address"",
    ""amount"": 100,
    ""date"": ""01-01-20 01:01:01 GMT""
}";
            var payoutJsonParsed = JObject.Parse(payoutJson);
            const string json = @"{
    ""status"": 200,
    ""message"": ""OK"",
    ""rewards"": []
}";

            var jsonParsed = JObject.Parse(json);
            for (int i = 0; i < count; i++)
            {
                ((JArray)jsonParsed["rewards"]).Add(payoutJsonParsed);
            }
            var finalJson = jsonParsed.ToString();

            // Act
            var client = new MockRequester("payouts", finalJson).CreateClient();
            var actualResult = await client.GetPayouts(count, FaucetPayClient.Bitcoin);

            expectedResult.ShouldMatch(actualResult);
        }

        [Theory]
        [InlineData(101)]
        [InlineData(-1)]
        public async Task GetPayouts_OutOfRange_ThrowsArgumentOutOfRangeException(int count)
        {
            var client = new MockRequester("payouts", null).CreateClient();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.GetPayouts(count, FaucetPayClient.Bitcoin));
        }

        private static IFaucetPayClient CreateCheckAddressClient(string json)
        {
            return new MockRequester("checkaddress", json).CreateClient();
        }

        private const string OkCheckAddress = @"{
    ""status"": 200,
    ""message"": ""OK"",
    ""payout_user_hash"": ""hash""
}";

        private const string OtherErrorCheckAddress = @"{
    ""status"": 403,
    ""message"": ""Invalid API key.""
}";

        private const string InvalidAddressCheckAddress = @"{
    ""status"": 456,
    ""message"": ""The address does not belong to any user.""
}";

        [Fact]
        public async Task TryCheckAddress_OK_HasCorrectData()
        {
            const string address = "some address";
            const string json = OkCheckAddress;
            var expectedResult = new TryCheckAddressResult(address, FaucetPayClient.Bitcoin, true, new CheckAddressResponse
            {
                PayoutUserHash = "hash"
            }).ToExpectedObject();

            var client = CreateCheckAddressClient(json);
            var actualResult = await client.TryCheckAddress(address, FaucetPayClient.Bitcoin);

            expectedResult.ShouldEqual(actualResult);
        }

        [Fact]
        public async Task TryCheckAddress_CheckFail_HasInvalidAddress()
        {
            const string address = "some address";
            const string json = InvalidAddressCheckAddress;

            var client = CreateCheckAddressClient(json);
            var actualResult = await client.TryCheckAddress(address, FaucetPayClient.Bitcoin);

            Assert.False(actualResult.IsValidAddress);
        }

        [Fact]
        public async Task TryCheckAddress_OtherError_ThrowsFaucetPaySharpException()
        {
            const string address = "some address";
            const string json = OtherErrorCheckAddress;

            var client = CreateCheckAddressClient(json);

            await Assert.ThrowsAsync<FaucetPaySharpException>(() => client.TryCheckAddress(address, FaucetPayClient.Bitcoin));
        }


        [Fact]
        public async Task CheckAddress_OK_HasCorrectData()
        {
            const string address = "some address";
            const string json = OkCheckAddress;
            var expectedResult = new CheckAddressResponse
            {
                PayoutUserHash = "hash"
            }.ToExpectedObject();

            var client = CreateCheckAddressClient(json);
            var actualResult = await client.CheckAddress(address, FaucetPayClient.Bitcoin);

            expectedResult.ShouldEqual(actualResult);
        }

        [Fact]
        public async Task CheckAddress_InvalidAddress_ThrowsInvalidCryptocurrencyAddressException()
        {
            const string address = "some address";
            const string json = InvalidAddressCheckAddress;

            var client = CreateCheckAddressClient(json);

            await Assert.ThrowsAsync<InvalidCryptocurrencyAddressException>(() => client.CheckAddress(address, FaucetPayClient.Bitcoin));
        }

        [Fact]
        public async Task CheckAddress_OtherError_ThrowsFaucetPaySharpException()
        {
            const string address = "some address";
            const string json = OtherErrorCheckAddress;

            var client = CreateCheckAddressClient(json);

            await Assert.ThrowsAsync<FaucetPaySharpException>(() => client.CheckAddress(address, FaucetPayClient.Bitcoin));
        }
    }
}