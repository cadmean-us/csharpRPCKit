using System;
using Xunit;

namespace Cadmean.RPC.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var rpc = new RpcServer("https://localhost:5001");
            ((DefaultFunctionUrlProvider) rpc.Configuration.FunctionUrlProvider).Prefix = "api/v1";
            var testResult = await rpc.Function("test").Call<int>(1, 1);
            
            Assert.Equal(2, testResult.Result);
        }
    }
}