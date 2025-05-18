using Api_entradas.Data;
using Api_entradas.DTOs;
using Api_entradas.Models;
using Api_entradas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MercadoPago.Client.Preference;
using MercadoPago.Client.Payment;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _email;
        private readonly IConfiguration _cfg;

        public TicketsController(AppDbContext db, IEmailService emailService, IConfiguration cfg)
        {
            _db = db;
            _email = emailService;
            _cfg = cfg;
        }

        // 1. Iniciar checkout
        [Authorize(Policy = "Client")]
        [HttpPost("{eventId}/purchase")]
        public async Task<IActionResult> Purchase(Guid eventId, [FromBody] PaymentDto dto)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null)
                return NotFound();
            if (ev.TicketsSold >= ev.TotalTickets)
                return BadRequest("Entradas agotadas.");

            // Configura la preferencia
            var preferenceRequest = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new()
                    {
                        Title     = ev.Title,
                        Quantity  = 1,
                        UnitPrice = dto.Price
                    }
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = _cfg["Mp:SuccessUrl"],
                    Failure = _cfg["Mp:FailureUrl"]
                },
                AutoReturn = "approved",
                ExternalReference = $"{eventId}|{User.FindFirstValue(ClaimTypes.NameIdentifier)}"
            };

            // Usar el cliente sin pasar el token (ya está global en MercadoPagoConfig)
            var prefClient = new PreferenceClient();
            var preference = await prefClient.CreateAsync(preferenceRequest);

            return Ok(new { checkoutUrl = preference.InitPoint });
        }

        // 2. Webhook de MercadoPago
        [HttpPost("webhook/mercadopago")]
        public async Task<IActionResult> MpWebhook([FromQuery] MpNotificationDto dto)
        {
            // 1. Parsear el payment_id a long
            if (!long.TryParse(dto.Id, out var paymentId))
                return BadRequest("ID de pago inválido.");

            // 2. Obtener detalle del pago
            var paymentClient = new PaymentClient();
            var payment = await paymentClient.GetAsync(paymentId);

            if (payment.Status != "approved")
                return BadRequest("Pago no aprobado.");

            // 3. ExternalReference puede venir como objeto; convertirlo a string
            var extRefObj = payment.ExternalReference;
            var extRef = extRefObj?.ToString() ?? "";
            var parts = extRef.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2
              || !Guid.TryParse(parts[0], out var eventId)
              || !Guid.TryParse(parts[1], out var userId))
            {
                return BadRequest("ExternalReference inválida.");
            }

            // 4. Actualizar stock
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound("Evento no encontrado.");

            ev.TicketsSold++;
            await _db.SaveChangesAsync();

            // 5. Generar QR y enviar correo
            var qr = QRCoderHelper.GenerateQr($"{eventId}|{userId}|{DateTime.UtcNow}");
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound("Usuario no encontrado.");

            await _email.SendEmailAsync(
                user.Email,
                "Tu entrada",
                $"<p>Tu entrada para {ev.Title}:</p>",
                qr
            );

            return Ok();
        }

    }
}
