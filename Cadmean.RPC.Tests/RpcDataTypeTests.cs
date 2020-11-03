using System.Collections.Generic;
using Xunit;

namespace Cadmean.RPC.Tests
{
    public class RpcDataTypeTests
    {
        [Fact]
        public void ShouldResolveIntDataType()
        {
            var o = 12;
            
            var expected = RpcDataType.Integer;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveFloatDataType()
        {
            var o = 12f;
            
            var expected = RpcDataType.Float;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveStringDataType()
        {
            var o = "12f";
            
            var expected = RpcDataType.String;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveListDataTypeFromArray()
        {
            var o = new [] {1, 2, 3};
            
            var expected = RpcDataType.List;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveListDataTypeFromList()
        {
            var o = new List<int> {1, 2, 3};
            
            var expected = RpcDataType.List;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveObjectDataType()
        {
            var o = new
            {
                a = 3,
                b = "adad"
            };
            
            var expected = RpcDataType.Object;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveAuthDataType()
        {
            var o = new JwtAuthorizationTicket("ad", "dsa");
            
            var expected = RpcDataType.AuthTicket;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ShouldResolveNullDataType()
        {
            object o = null;
            
            var expected = RpcDataType.Null;
            var actual = RpcDataType.ResolveRpcDataType(o);
            
            Assert.Equal(expected, actual);
        }
    }
}