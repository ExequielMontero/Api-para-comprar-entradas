using Api_entradas.Data;
using Api_entradas.DTOs;
using Api_entradas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EventsController(AppDbContext db) => _db = db;

        [Authorize(Policy = "Organizer")]
        [HttpPost]
        public async Task<IActionResult> Create(EventDto dto)
        {
            var ev = new Event
            {
                Title = dto.Title,
                Date = dto.Date,
                TotalTickets = dto.TotalTickets,
                TicketsSold = 0
            };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            return CreatedAtAction(null, new { ev.Id }, ev);
        }

        [Authorize(Policy = "Organizer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var total = await _db.Events.CountAsync();
            var items = await _db.Events
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new {
                    e.Id,
                    e.Title,
                    e.Date,
                    e.TotalTickets,
                    e.TicketsSold,
                    IsSoldOut = e.TicketsSold >= e.TotalTickets
                })
                .ToListAsync();

            return Ok(new PageResult<object>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            });
        }
    }
}
