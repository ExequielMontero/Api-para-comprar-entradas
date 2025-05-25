using Api_entradas.DTOs;
using Api_entradas.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService authService)
            => _auth = authService;



        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(new { Finalidad = "Utilizar el digito segun el rol a elegir para registrarse", Detalles = "Cliente = 0, Organizador = 1, Admin = 2" });
        }

        // Otros endpoints (login, registro, etc.)


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var user = await _auth.RegisterAsync(dto.User, dto.Email, dto.Password, dto.FechaNacimiento);
                return CreatedAtAction(
                actionName: "GetUserById",
                controllerName: "User",
                routeValues: new { id = user.Id },
                value: new { user.Id, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _auth.ValidateUserAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized("Credenciales inválidas.");

            var token = await _auth.GenerateTokenAsync(user);
            return Ok(new { token });
        }
    }
}
