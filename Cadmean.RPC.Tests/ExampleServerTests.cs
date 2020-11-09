using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class ExampleServerTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RpcClient rpc;

        public ExampleServerTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            rpc = new RpcClient("http://testrpc.cadmean.ru");
        }

        [Fact]
        public async Task ShouldCallSum()
        {
            var a = 1;
            var b = 2;
            var expected = 3;
            var actual = await rpc.Function("sum").CallThrowing<int>(a, b);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ShouldCallSquare()
        {
            var a = 0.3;
            var expected = 0.09;
            var actual = await rpc.Function("square").CallThrowing<float>(a);
            Assert.Equal(expected, actual, 3);
        }

        [Fact]
        public async Task ShouldCallConcat()
        {
            var a = "Hello, ";
            var b = "RPC!";
            var expected = "Hello, RPC!";
            var actual = await rpc.Function("concat").CallThrowing<string>(a, b);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ShouldCallGetDate()
        {
            var actual = await rpc.Function("getDate").CallThrowing<DateTime>();
            testOutputHelper.WriteLine(actual.ToString(CultureInfo.InvariantCulture));
        }


        public struct AuthUser
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
        
        [Fact]
        public async Task ShouldCallAuthorizedFunction()
        {
            var expected = new AuthUser
            {
                Email = "email@example.com",
                Name = "George",
            };
            
            await rpc.Function("auth").CallThrowing("email@example.com", "password");
            var actual = await rpc.Function("user.get").CallThrowing<AuthUser>();
            Assert.Equal(expected, actual);
        }
    }
}