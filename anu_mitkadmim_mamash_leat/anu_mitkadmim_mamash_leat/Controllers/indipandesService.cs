using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using anu_mitkadmim_mamash_leat.Data;
using anu_mitkadmim_mamash_leat.Models;
using anu_mitkadmim_mamash_leat.Controllers;
using anu_mitkadmim_mamash_leat.hub;
using Microsoft.AspNetCore.SignalR;

namespace anu_mitkadmim_mamash_leat.Controllers
{
    [Authorize]
    public class indipandesService : Controller
    {
        private readonly anu_mitkadmim_mamash_leatContext _context;
        private readonly IHubContext<Class> hub;

        public indipandesService(anu_mitkadmim_mamash_leatContext context, IHubContext<Class> _hub)
        {
            hub = _hub;
            _context = context;
        }
        // POST: Contacts/:id/messages
        public async Task<IActionResult> invitations(string From, string to, string server)
        {
            var userFrom = await (from c in _context.User where c.id == to select c).ToListAsync();
            if (userFrom.Count() == 0)
            {
                return NotFound();
            }
            var con = await (from c in _context.Contact where c.id == From select c).ToListAsync();
            if (con.Count() == 0)
            {
                Contact contactTo = new Contact();
                contactTo.userid = to;
                contactTo.server = server;
                contactTo.lastdate = null;
                contactTo.id = From;
                contactTo.name = From;
                contactTo.last = null;
                _context.Contact.Add(contactTo);
                await _context.SaveChangesAsync();
                Chat c = new Chat();
                c.contact = contactTo;
                c.userid = to;
                _context.Chat.Add(c);
                await _context.SaveChangesAsync();
            }
            
            //Chat chat = new Chat();
            //chat.userid = userFrom.First().id;
            //chat.contact = contactTo;
            //_context.Chat.Add(chat);
            //await _context.SaveChangesAsync();
            await hub.Clients.All.SendAsync("recive_contact: " + to, new { contact_id = From, server = "6132" });
            return NoContent();
        }

        public async Task<IActionResult> transfer(string From, string to, string content)
        {
            var conlist = await (from c in _context.Contact where c.id == to select c).ToListAsync();
            if (conlist.Count() == 0)
            {
                return BadRequest();
            }
            var con = conlist.First();
            if (con.server != "6132")
            {
                var userFrom = await (from c in _context.User where c.id == From select c).ToListAsync();
                if (userFrom.Count == 0)
                {
                    return NotFound();
                }
                var contact = await (from c in _context.Contact where c.id == to select c).ToListAsync();
                if (contact.Count == 0)
                {
                    return NotFound();
                }
                var chat = await (from c in _context.Chat where c.contact == contact.First() && c.userid == userFrom.First().id select c).ToListAsync();
                if (chat.Count == 0)
                {
                    return NotFound();
                }
                Message message = new Message();
                message.content = content;
                message.created = DateTime.Now;
                message.sent = true;
                message.chat_id = chat.First().id;
                _context.Message.Add(message);
                await _context.SaveChangesAsync();
                await hub.Clients.All.SendAsync("recive_message: " + to, new { contact_id = From, ContentMessege = content, time = DateTime.Now, chat_id = chat.First().id });
                return NoContent();
            }
            else
                return Ok(con);
        }
    }
}