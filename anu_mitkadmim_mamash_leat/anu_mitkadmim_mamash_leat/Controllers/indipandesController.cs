using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using anu_mitkadmim_mamash_leat.Data;
using anu_mitkadmim_mamash_leat.hub;
using Microsoft.AspNetCore.SignalR;

namespace anu_mitkadmim_mamash_leat.Controllers
{
    [ApiController]
    [Route("api/")]
    public class indipandesController : Controller
    {
        private readonly indipandesService service;

        public indipandesController(anu_mitkadmim_mamash_leatContext context, IHubContext<Class> _hub)
        {
            service = new indipandesService(context, _hub);
        }
        public class neri
        {
            public string From { get; set; }

            public string to { get; set; }

            public string server { get; set; }
        }
        // POST: Contacts/:id/messages
        [HttpPost("invitations")]
        public async Task<IActionResult> invitations([Bind("From", "to", "server")] neri ner)
        {

            return await service.invitations(ner.From, ner.to, ner.server);
        }

        [HttpPost("transfer")]

        public async Task<IActionResult> transfer([Bind("From", "to", "server")] neri ner)
        {

            return await service.transfer(ner.From, ner.to, ner.server);
        }
    }
}
