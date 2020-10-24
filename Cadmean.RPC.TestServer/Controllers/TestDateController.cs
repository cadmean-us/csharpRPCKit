using System;
using Cadmean.RPC.ASP;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [ApiController]
    [Route("api/rpc/test.getDate")]
    public class TestDateController : FunctionController
    {
        public DateTime OnCall()
        {
            return new DateTime(2020, 10, 24, 22, 15, 0);
        }
    }
}