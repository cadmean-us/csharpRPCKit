using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class JsonCodecProviderTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private JsonCodecProvider jsonCodecProvider = new JsonCodecProvider();

        public struct Data
        {
            public string Name;
            public int Nice;
        }
        
        private readonly Data data = new Data
        {
            Name = "Bru",
            Nice = 69
        };

        public JsonCodecProviderTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldEncode()
        {
            var encoded = jsonCodecProvider.Encode(data);
            testOutputHelper.WriteLine(Encoding.UTF8.GetString(encoded));
        }
        
        [Fact]
        public void ShouldDecode()
        {
            var expected = data;
            var encoded = jsonCodecProvider.Encode(data);
            testOutputHelper.WriteLine(Encoding.UTF8.GetString(encoded));
            var actual = jsonCodecProvider.Decode(encoded, data.GetType());
            Assert.Equal(expected, actual);
        }
    }
}