using System.Threading.Tasks;
using Cadmean.RPC.ASP;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("bru")]
    public class BruController : FunctionController
    {
        public class MyStruct
        {
            public string Name { get; set; }
            public int Nice { get; set; }
        }

        private Task<MyStruct> OnCall()
        {
            // throw new FunctionException(69);
            return Task.FromResult(new MyStruct
            {
                Name = "Georg",
                Nice = 69,
            });
        }
    }
}