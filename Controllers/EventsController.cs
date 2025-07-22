using Api_entradas.Data;
using Api_entradas.DTOs;
using Api_entradas.Models;
using Api_entradas.Services;
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

        private readonly CloudinaryService _cloudinaryService;
        public EventsController(AppDbContext db, CloudinaryService cloudinary)
        {
            _db = db;
            _cloudinaryService = cloudinary;
        }


        [Authorize(Policy = "Organizador")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateEventWithBannerDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );
                    return BadRequest(new { mensaje = "Error de validación", errores = errors });
                }

                if (dto.Banner == null || dto.Banner.Length == 0)
                {
                    return BadRequest(new { mensaje = "El banner es requerido" });
                }

                string url;
                try
                {
                    url = await _cloudinaryService.UploadImageAsync(dto.Banner);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { mensaje = "Error al subir imagen", error = ex.Message });
                }

                var ev = new Evento
                {
                    Title = dto.Title,
                    BannerEvento = url,
                    Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                    TotalTickets = dto.TotalTickets,
                    Price = dto.Price,
                    TicketsSold = 0
                };

                try
                {
                    _db.Events.Add(ev);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(500, new { mensaje = "Error al guardar el evento", error = ex.InnerException?.Message ?? ex.Message });
                }

                var eventResponse = new {
                    ev.Id,
                    ev.Title,
                    Date = ev.Date.ToString("yyyy-MM-dd"),
                    ev.TotalTickets,
                    ev.TicketsSold,
                    IsSoldOut = ev.TicketsSold >= ev.TotalTickets,
                    ev.BannerEvento
                };

                return CreatedAtAction(nameof(GetAll), new { ev.Id }, eventResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
            }
        }




        [Authorize(Policy = "Organizador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var ev = await _db.Events.FindAsync(id);
                if (ev == null) return NotFound(new { mensaje = "Evento no encontrado" });

                _db.Events.Remove(ev);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el evento", error = ex.Message });
            }
        }

        // [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1)
                    return BadRequest(new { mensaje = "El número de página debe ser mayor a 0" });

                if (pageSize < 1)
                    return BadRequest(new { mensaje = "El tamaño de página debe ser mayor a 0" });

                var total = await _db.Events.CountAsync();
                var items = await _db.Events
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new {
                        e.Id,
                        e.Title,
                        Date = e.Date.Date.ToString("yyyy-MM-dd"),
                        e.TotalTickets,
                        e.TicketsSold,
                        IsSoldOut = e.TicketsSold >= e.TotalTickets,
                        e.BannerEvento
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
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los eventos", error = ex.Message });
            }
        }
    }
}
