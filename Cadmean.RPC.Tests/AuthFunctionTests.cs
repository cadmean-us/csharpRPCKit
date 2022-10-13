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
            const string email = "email@example.com";
            const string pass = "password";
            
            var expected = new JwtAuthorizationTicket("access", "refresh");
            const string expectedResultType = "Cadmean.RPC.JwtAuthorizationTicket";
            
            var output = await rpc.Function("test.auth").Call<JwtAuthorizationTicket>(email, pass);
            
            Assert.True(string.IsNullOrEmpty(output.Error));
            
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
            const string pass = "password";
            
            const string expectedError = "invalid_credentials";
            
            var output = await rpc.Function("test.auth").Call<JwtAuthorizationTicket>(email, pass);
            testOutputHelper.WriteLine(output.Error);
            
            Assert.Equal(expectedError, output.Error);
        }

        [Fact]
        public async void ShouldAuthenticateAndCallTestFunction_GetUserAuth()
        {
            const string email = "email@example.com";
            const string pass = "password";
            
            var expected = new PocoRpcFunctionTests.User
            {
                Name = "Georg",
                Surname = "Kot",
                Age = 69
            };

            var output = await rpc.Function("test.auth2").Call(email, pass);
            Assert.True(string.IsNullOrEmpty(output.Error));
            
            var output1 = await rpc.Function("test.getUserAuth").Call<PocoRpcFunctionTests.User>(email);
            Assert.True(string.IsNullOrEmpty(output1.Error));
            var actual = output1.Result;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void ShouldReturnErrorWithoutAuthorization()
        {
            const string email = "email@example.com";

            var expectedError = RpcErrorCode.AuthorizationError.Description();

            var output = await rpc.Function("test.getUserAuth").Call<PocoRpcFunctionTests.User>(email);
            
            Assert.Equal(expectedError, output.Error);
        }
    }
}