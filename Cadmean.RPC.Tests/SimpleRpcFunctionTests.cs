using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class SimpleRpcFunctionTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RpcClient rpc;

        public SimpleRpcFunctionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            rpc = new RpcClient("https://localhost:5001");
        }


        [Fact]
        public async void ShouldCallTestFunction_AddInt()
        {
            const int a = 1;
            const int b = 68;
            const int expected = 69;
            const int expectedError = 0;
            var output = await rpc.Function("test.addInt").Call<int>(a, b);
            Assert.Equal(expectedError, output.Error);
            var actual = output.Result;
            testOutputHelper.WriteLine(actual.ToString());
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public async void ShouldCallTestFunction_SquareDouble()
        {
            const int a = 3;
            const int expected = 9;
            const int expectedError = 0;
            var output = await rpc.Function("test.squareDouble").Call<double>(a);
            Assert.Equal(expectedError, output.Error);
            var actual = output.Result;
            testOutputHelper.WriteLine(actual.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public async void ShouldCallTestFunction_ConcatString()
        {
            const string a = "Hello, ";
            const string b = "cadRPC";
            const string c = "!";
            const string expected = "Hello, cadRPC!";
            const int expectedError = 0;
            var output = await rpc.Function("test.concatString").Call<string>(a, b, c);
            Assert.Equal(expectedError, output.Error);
            var actual = output.Result;
            testOutputHelper.WriteLine(actual);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public async void ShouldCallTestFunction_InvertBool()
        {
            const bool b = true;
            const bool expected = false;
            const int expectedError = 0;
            var output = await rpc.Function("test.invertBool").Call<bool>(b);
            Assert.Equal(expectedError, output.Error);
            var actual = output.Result;
            testOutputHelper.WriteLine(actual.ToString());
            Assert.Equal(expected, actual);
        }
    }
}