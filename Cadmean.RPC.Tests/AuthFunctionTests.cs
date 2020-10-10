using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class AuthFunctionTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RpcClient rpc;

        public AuthFunctionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            rpc = new RpcClient("https://localhost:5001");
        }

        [Fact]
        public async void ShouldCallTestFunction_Auth()
        {
            const string email = "krit.allyosha@gmail.com";
            const string pass = "bruh";
           
            const int expectedError = 0;
            var expected = new JwtAuthorizationTicket("access", "refresh");
            const string expectedResultType = "Cadmean.RPC.JwtAuthorizationTicket";
            
            var output = await rpc.Function("test.auth").Call<JwtAuthorizationTicket>(email, pass);
            
            Assert.Equal(expectedError, output.Error);
            
            var actual = output.Result;
            testOutputHelper.WriteLine(actual.ToString());
            
            Assert.Equal(expected, actual);
            Assert.NotNull(output.MetaData);
            Assert.True(output.MetaData.ContainsKey("clrResultType"));
            Assert.Equal(expectedResultType, output.MetaData["clrResultType"]);
            Assert.Equal(expected, rpc.Configuration.AuthorizationTicketHolder.Ticket);
        }
        
        [Fact]
        public async void ShouldThrowOnCallTestFunction_Auth()
        {
            const string email = "test@test.test";
            const string pass = "test";
            
            const int expectedError = 101;
            
            var output = await rpc.Function("test.auth").Call<JwtAuthorizationTicket>(email, pass);
            testOutputHelper.WriteLine(output.Error.ToString());
            
            Assert.Equal(expectedError, output.Error);
        }

        [Fact]
        public async void ShouldAuthenticateAndCallTestFunction_GetUserAuth()
        {
            const string email = "krit.allyosha@gmail.com";
            const string pass = "bruh";
            
            var expected = new PocoRpcFunctionTests.User
            {
                Name = "Georg",
                Surname = "Kot",
                Age = 69
            };
            const int expectedError = 0;
            
            var output = await rpc.Function("test.auth2").Call(email, pass);
            Assert.Equal(expectedError, output.Error);
            
            var output1 = await rpc.Function("test.getUserAuth").Call<PocoRpcFunctionTests.User>(email);
            Assert.Equal(expectedError, output1.Error);
            var actual = output1.Result;
            Assert.Equal(expected, actual);
        }
    }
}