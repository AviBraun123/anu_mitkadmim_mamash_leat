using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using anu_mitkadmim_mamash_leat.Models;
using anu_mitkadmim_mamash_leat.Data;

namespace anu_mitkadmim_mamash_leat.Controllers
{
    [Authorize]
    public class serviceContacts : Controller
    {
        private readonly anu_mitkadmim_mamash_leatContext _context;
        public serviceContacts(anu_mitkadmim_mamash_leatContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> getall(string a)
        {
            var res2 = await (_context.Chat.Include(x => x.contact).Where(y => y.userid == a).Select(c => c)).ToListAsync();
            //return Json(await _context.Contact.ToListAsync());
           
            return Json(res2);
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> getby(string? id)
        {
            if (id == null || _context.Contact == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .FirstOrDefaultAsync(m => m.id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return Json(contact);
        }

        // POST: Contacts/Create
        public async Task<IActionResult> add(string id, string name, string server, string a)
        {
            if (ModelState.IsValid)
            {
                Contact contact = new Contact();
                contact.userid = a;
                contact.id = id;
                contact.name = name;
                contact.server = server;
                contact.lastdate = null;
                contact.last = null;
                Chat c = new Chat();
                c.contact = contact;
                c.userid = a;
                _context.Contact.Add(contact);
                _context.Chat.Add(c);
                await _context.SaveChangesAsync();
                return Created(string.Format("/api/Contacts/{0}", contact.id), contact);
            }
            return BadRequest();
        }

        // POST: Contacts/Edit/5
        public async Task<IActionResult> edit(string name, string server, string id)
        {
            var contact = await (from d in _context.Contact where d.id == id select d).ToListAsync();
            if (contact.Count == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    contact.First().name = name;
                    contact.First().server = server;
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.First().id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            return BadRequest();
        }

        // delete: Contacts/Delete/5
        public async Task<IActionResult> deletecon(string id, string a)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact != null)
            {
                _context.Contact.Remove(contact);
                var cc = await (from c in _context.Chat where c.userid == a && c.contact.id == id select c).ToListAsync();
                _context.Chat.Remove(cc.First());
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        private bool ContactExists(string id)
        {
            return _context.Contact.Any(e => e.id == id);
        }

        // GET: Contacts/:id/messages
        public async Task<IActionResult> chatWith(string id, string a)
        {
            if (id == null || _context.Contact == null)
            {
                return NotFound();
            }
            var cid = await (from c in _context.Chat where c.userid == a && c.contact.id == id select c.id).ToListAsync();
            if (cid.Count() == 0)
            {
                return NotFound();
            }
            var mess = await (from d in _context.Message where d.chat_id == cid.First() select d).ToListAsync();
            if (mess.Count() == 0)
            {
                return NotFound();
            }

            return Json(mess);
        }

        // POST: Contacts/:id/messages
        public async Task<IActionResult> CreateMess(string id, string content, string a)
        {
            var conlist = await (from c in _context.Contact where c.id == id select c).ToListAsync();
            if(conlist.Count() == 0)
            {
                return BadRequest();
            }
            var con = conlist.First();
            if (con.server == "6132")
            {
                if (ModelState.IsValid)
                {
                    Message message = new Message();
                    message.created = DateTime.Now;
                    var cid = await (from c in _context.Chat where c.userid == a && c.contact.id == id select c).ToListAsync();
                    if (cid.Count() == 0)
                    {
                        return NotFound();
                    }
                    message.chat_id = cid.First().id;
                    message.content = content;
                    cid.First().contact.lastdate = DateTime.Now;
                    cid.First().contact.last = content;
                    _context.Message.Add(message);
                    await _context.SaveChangesAsync();
                    Message message1 = new Message();
                    message1.created = DateTime.Now;
                    var cid1 = await (from c in _context.Chat where c.userid == id && c.contact.id == a select c).ToListAsync();
                    if (cid.Count() == 0)
                    {
                        return NotFound();
                    }
                    message1.chat_id = cid1.First().id;
                    message1.content = content;
                    cid1.First().contact.lastdate = DateTime.Now;
                    cid1.First().contact.last = content;
                    _context.Message.Add(message1);
                    await _context.SaveChangesAsync();
                    return Created(string.Format("/api/Contacts/{0}/messages/{1}", id, message.id), message);
                }
                return BadRequest();
            }
            else
                return Ok(con);
        }

        // GET: Contacts/:id/messages/:id2
        public async Task<IActionResult> getMessage(string id, int id2, string a)
        {
            var cid = await (from c in _context.Chat where c.userid == a && c.contact.id == id select c.id).ToListAsync();
            if (cid.Count == 0)
            {
                return NotFound();
            }

            var mess = await (from d in _context.Message where d.chat_id == cid.First() select d).ToListAsync();
            if (mess.Count == 0)
            {
                return NotFound();
            }
            var b = await (from e in _context.Message where e.id == id2 select e).ToListAsync();
            if (b.Count() == 0)
            {
                return NotFound();
            }
            var outmess = b.First();
            if (!mess.Contains(outmess))
            {
                return NotFound();
            }
            return Json(outmess);
        }

        // put: Contacts/:id/messages/:id2
        public async Task<IActionResult> putMessage(string id, int id2, string content, string a)
        {
            var cid = await (from c in _context.Chat where c.userid == a && c.contact.id == id select c.id).ToListAsync();
            if (cid.Count() == 0)
            {
                return NotFound();
            }

            var mess = await (from d in _context.Message where d.chat_id == cid.First() select d).ToListAsync();
            if (mess.Count() == 0)
            {
                return NotFound();
            }
            var outmes = await (from e in _context.Message where e.id == id2 select e).ToListAsync();
            if (outmes.Count() == 0)
            {
                return NotFound();
            }
            var outmess = outmes.First();
            if (!mess.Contains(outmess))
            {
                return NotFound();
            }
            outmess.content = content;
            outmess.created = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(outmess);
        }
        public async Task<IActionResult> DeleteMess(string id, int id2, string a)
        {
            var cid = await (from c in _context.Chat where c.userid == a && c.contact.id == id select c.id).ToListAsync();
            if (cid.Count() == 0)
            {
                return NotFound();
            }

            var mess = await (from d in _context.Message where d.chat_id == cid.First() select d).ToListAsync();
            if (mess.Count() == 0)
            {
                return NotFound();
            }
            var outmes = await (from e in _context.Message where e.id == id2 select e).ToListAsync();
            if (outmes.Count() == 0)
            {
                return NotFound();
            }
            var outmess = outmes.First();
            if (!mess.Contains(outmess))
            {
                return NotFound();
            }
            mess.Remove(outmess);
            _context.Message.Remove(outmess);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    //class nosaf
}