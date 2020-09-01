using System;
using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void Test1()
        {
            var rpc = new RpcServer("https://localhost:5001");
            ((DefaultFunctionUrlProvider) rpc.Configuration.FunctionUrlProvider).Prefix = "api/v1";
            testOutputHelper.WriteLine(rpc.Configuration.FunctionUrlProvider.GetUrl(rpc.Function("test")));
            var testResult = await rpc.Function("test").Call<int>(1, 1);
            
            Assert.Equal(2, testResult.Result);
        }
    }
}