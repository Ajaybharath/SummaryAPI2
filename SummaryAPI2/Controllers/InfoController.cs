using SummaryAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SummaryAPI2.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("API/Info")]
    public class InfoController : ApiController
    {
        [Route("Clients")]
        [HttpPost]
        public dynamic getClientData(Client cl)
        {
            return "hi";
        }
    }
}
