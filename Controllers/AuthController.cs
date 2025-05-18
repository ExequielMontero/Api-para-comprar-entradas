using Api_entradas.DTOs;
using Api_entradas.Enums;
using Api_entradas.Models;
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
            return Ok(new {Finalidad = "Utilizar el digito segun el rol a elegir para registrarse",Detalles = "Cliente = 0, Organizador = 1, Admin = 2" });
        }

        // Otros endpoints (login, registro, etc.)


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _auth.RegisterAsync(dto.Email, dto.Password);
            return CreatedAtAction(null, new { user.Id }, new { user.Id, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _auth.ValidateUserAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized("Credenciales inválidas.");

            var token = await _auth.GenerateTokenAsync(user);
            return Ok(new { token });
        }
    }
}
