using Api_entradas.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController
    {

        [HttpPost("register")] public async Task<IActionResult> Register(RegisterDto dto) { /* hashear, guardar user con rol Client */ }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Validar credenciales, generar JWT, retornar { token }
        }
    }
