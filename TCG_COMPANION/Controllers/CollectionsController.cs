using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;
using TCG_COMPANION.Models;

namespace TCG_COMPANION.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private readonly CollectionsContext _context;

        public CollectionsController(CollectionsContext context)
        {
            _context = context;
        }

        // GET: api/Collections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collections>>> GetCollections()
        {
            return await _context.Collections.ToListAsync();
        }

        // GET: api/Collections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Collections>> GetCollections(int id)
        {
            var collections = await _context.Collections.FindAsync(id);

            if (collections == null)
            {
                return NotFound();
            }

            return collections;
        }

        // PUT: api/Collections/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]         public async Task<IActionResult> PutCollections(int id, Collections collections)
        {
            if (id != collections.Id)
            {
                return BadRequest();
            }

            _context.Entry(collections).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionsExists(id))
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

        // POST: api/Collections
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Collections>> PostCollections(Collections collections)
        {
            _context.Collections.Add(collections);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCollections", new { id = collections.Id }, collections);
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollections(int id)
        {
            var collections = await _context.Collections.FindAsync(id);
            if (collections == null)
            {
                return NotFound();
            }

            _context.Collections.Remove(collections);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CollectionsExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }
    }
}
