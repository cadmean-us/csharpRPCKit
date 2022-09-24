using System;
using Cadmean.RPC.ASP;
using Cadmean.RPC.ASP.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.TestServer.Controllers
{
    [FunctionRoute("test.getDate")]
    public class TestDateController : FunctionController
    {
        public DateTime OnCall()
        {
            return new DateTime(2020, 10, 24, 22, 15, 0);
        }
    }
}