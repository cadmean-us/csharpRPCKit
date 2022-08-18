using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests;

public class WsSubscriberTest
{
    private readonly ITestOutputHelper testOutputHelper;
    
    public WsSubscriberTest(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        
    }
    
    [Fact]
    public async Task ShouldSubscribe()
    {
        var s = new WsFunctionSubscriber();
        await s.Subscribe<string>("localhost:5000", "test", testOutputHelper.WriteLine);
    }
}