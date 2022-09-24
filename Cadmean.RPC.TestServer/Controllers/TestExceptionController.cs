using System.IO;
using Cadmean.RPC.ASP;
using Cadmean.RPC.ASP.Attributes;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("test.getException")]
    public class TestExceptionController : FunctionController
    {
        public string OnCall()
        {
            throw new InvalidDataException();
        }
    }
}