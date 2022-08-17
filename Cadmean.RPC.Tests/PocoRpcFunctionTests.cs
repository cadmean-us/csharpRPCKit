using System;
using Xunit;
using Xunit.Abstractions;

namespace Cadmean.RPC.Tests
{
    public class PocoRpcFunctionTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RpcClient rpc;

        public PocoRpcFunctionTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            rpc = new RpcClient("https://localhost:5001");
        }

        public struct User
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }

            public bool Equals(User other)
            {
                return Name == other.Name && Surname == other.Surname && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                return obj is User other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Surname, Age);
            }

            public override string ToString()
            {
                return $"{Name} {Surname} {Age}";
            }
        }

        [Fact]
        public async void ShouldCallTestFunction_GreetUser()
        {
            var u = new User
            {
                Name = "Georg",
                Surname = "Kot",
                Age = 69
            };
            const string expected = "Hello, Georg69";
            var output = await rpc.Function("test.greetUser").Call<string>(u);
            Assert.True(string.IsNullOrEmpty(output.Error));
            var actual = output.Result;
            testOutputHelper.WriteLine(actual);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public async void ShouldCallTestFunction_GetUser()
        {
            var expected = new User
            {
                Name = "Alex",
                Surname = "Krit",
                Age = 42
            };
            var output = await rpc.Function("test.getUser").Call<User>();
            Assert.True(string.IsNullOrEmpty(output.Error));
            var actual = output.Result;
            testOutputHelper.WriteLine(actual.ToString());
            Assert.Equal(expected, actual);
        }
    }
}