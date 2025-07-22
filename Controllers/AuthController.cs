using Api_entradas.Data;
using Api_entradas.DTOs;
using Api_entradas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly AppDbContext _context;
        public AuthController(AuthService authService, AppDbContext db)
        {
         _auth = authService;
          _context = db;

        }



        /// <summary>
        /// Endpoint para ver los tipos de roles en la app
        /// </summary>
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(new { Finalidad = "Utilizar el digito segun el rol a elegir para registrarse", Detalles = "Cliente = 0, Organizador = 1, Admin = 2" });
        }

        // Otros endpoints (login, registro, etc.)

        /// <summary>
        /// Endpoint para Registrar una cuenta. Rol por defecto 0(Cliente).
        /// </summary>
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
                routeValues: new { user = user.User},
                value: new { user.Id, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Endpoint para loguearse si antes ya te hiciste tu cuenta.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _auth.ValidateUserAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized("Credenciales inválidas.");

            var token = await _auth.GenerateTokenAsync(user);

            var userInfo = new
            {
                id = user.Id,
                email = user.Email,
                username = user.User,
                role = user.Role
            };

            return Ok(new { token, user = userInfo });
        }
        




        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.User == request.User);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var nuevoHash = _auth.HashPassword(request.Password);
            user.PasswordHash = nuevoHash;

            _context.SaveChanges();

            return Ok("Contraseña restablecida correctamente");
        }
    }
}
