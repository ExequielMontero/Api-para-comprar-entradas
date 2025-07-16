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



        [Authorize(Policy = "Organizador")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateEventWithBannerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Banners");
            Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Banner.FileName);
            var savePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await dto.Banner.CopyToAsync(stream);
            }

            var url = $"{Request.Scheme}://{Request.Host}/banners/{fileName}";

            var ev = new Evento
            {
                Title = dto.Title,
                BannerEvento = url,
                Date = dto.Date,
                TotalTickets = dto.TotalTickets,
                Price = dto.Price,
                TicketsSold = 0
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return CreatedAtAction(null, new { ev.Id }, ev);
        }



        [Authorize(Policy = "Organizador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // [AllowAnonymous]
        [Authorize]
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
