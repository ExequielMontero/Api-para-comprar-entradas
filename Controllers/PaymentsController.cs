using Microsoft.AspNetCore.Mvc;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        [HttpGet("success")]
        public IActionResult Success()
        => Content("<h1>¡Pago aprobado!</h1><p>Gracias por tu compra.</p>", "text/html");

        [HttpGet("failure")]
        public IActionResult Failure()
            => Content("<h1>Pago rechazado o cancelado</h1><p>Intenta de nuevo.</p>", "text/html");
    }
}
