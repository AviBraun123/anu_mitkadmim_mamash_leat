using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using anu_mitkadmim_mamash_leat.Data;
using anu_mitkadmim_mamash_leat.Models;
using Microsoft.AspNetCore.Authorization;

namespace anu_mitkadmim_mamash_leat.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class ContactsController : Controller
    {
        private readonly serviceContacts service;
        public ContactsController(anu_mitkadmim_mamash_leatContext context)
        {
            service = new serviceContacts(context);
        }

        // GET: Contacts
        [HttpGet()]
        public async Task<IActionResult> getall()
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.getall(a);
        }

        // GET: Contacts/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> getby(string? id)
        {
            return await service.getby(id); ;
        }

        public class avi
        {
            public string id { get; set; }

            public string name { get; set; }

            public string server { get; set; }
        }

        // POST: Contacts/Create
        [HttpPost()]
        public async Task<IActionResult> add([Bind("id", "name", "server")] avi av)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.add(av.id, av.name, av.server, a);
        }

        // POST: Contacts/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> edit(string name, string server, string id)
        {
            return await service.edit(name, server, id);
        }

        // delete: Contacts/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> deletecon(string id)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.deletecon(id, a);
        }

        // GET: Contacts/:id/messages
        [HttpGet("{id}/messages")]
        public async Task<IActionResult> chatWith(string id)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.chatWith(id, a);
        }

        // POST: Contacts/:id/messages
        [HttpPost("{id}/messages")]
        public async Task<IActionResult> CreateMess(string id, string content)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.CreateMess(id, content, a);
        }

        // GET: Contacts/:id/messages/:id2
        [HttpGet("{id}/messages/{id2}")]
        public async Task<IActionResult> getMessage(string id, int id2)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.getMessage(id, id2, a);
        }

        // put: Contacts/:id/messages/:id2
        [HttpPut("{id}/messages/{id2}")]
        public async Task<IActionResult> putMessage(string id, int id2, string content)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.putMessage(id, id2, content, a);
        }

        [HttpDelete("{id}/messages/{id2}")]
        public async Task<IActionResult> DeleteMess(string id, int id2)
        {
            var a = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            return await service.DeleteMess(id, id2, a);
        }

    }

    //class nosaf
}