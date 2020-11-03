using System.IO;
using Cadmean.RPC.ASP;

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