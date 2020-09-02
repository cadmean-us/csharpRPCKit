using System.Threading.Tasks;
using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("/api/v1/bru")]
    public class BruController : FunctionController
    {
        public class MyStruct
        {
            public string Name { get; set; }
            public int Nice { get; set; }
        }

        public Task<MyStruct> OnCall()
        {
            return Task.FromResult(new MyStruct
            {
                Name = "Georg",
                Nice = 69,
            });
        }
    }
}