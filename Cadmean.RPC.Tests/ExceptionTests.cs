using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class ExceptionTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RpcClient rpc;

        public ExceptionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            rpc = new RpcClient("https://localhost:5001");
        }

        // private async void ShouldThrowExceptionWhenServerNotResponding()
        // {
        //     
        // }
    }
}