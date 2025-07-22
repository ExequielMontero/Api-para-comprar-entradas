using Api_entradas.Data;
using Api_entradas.DTOs;
using Api_entradas.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Identity.Data;
namespace Api_entradas.Controllers
{
    /// <summary>
    /// Controlador para gestion de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista de todos los usuarios. Acceso solo para Administradores.
        /// </summary>
        // GET: api/users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {

            try
            {
                var users = await _context.User
                    .Select(u => new {
                        u.Id,
                        u.User,
                        u.Email,
                        u.Role,
                        FechaNacimiento = u.FechaNacimiento.ToString("yyyy-MM-dd"),
                        u.EstaVerificado
                    })
                    .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Luego puedo loguear el error si uso ILogger, por ahora se retorna un 500 genérico
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un usuario por su ID. Acceso solo para Administradores.
        /// </summary>
        // GET: api/users/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{user}")]
        public async Task<IActionResult> GetUserById(string user)
        {
            try
            {
                var usuario = await _context.User
                    .Select(u => new {
                        u.Id,
                        u.User,
                        u.Email,
                        u.Role,
                        FechaNacimiento = u.FechaNacimiento.ToString("yyyy-MM-dd"),
                        u.EstaVerificado
                    })
                    .FirstAsync(x => x.User == user);
                if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado" });
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                // Luego puedo loguear el error si uso ILogger, por ahora se retorna un 500 genérico
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Permite actualizar el atributo que se requiera o todos del usuario. Acceso para usuarios autenticado.
        /// </summary>
        [SwaggerRequestExample(typeof(List<Operation>), typeof(PatchUserRequestExample))]

        [Consumes("application/json-patch+json")]
        [Authorize]
        [HttpPatch("{usuario}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string usuario, [FromBody] JsonPatchDocument<UpdateUserDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var user = await _context.User.FirstOrDefaultAsync(x => x.User == usuario);
            if (user == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            // Mapea el usuario actual a un DTO
            var userDto = new UpdateUserDto
            {
                Usuario = user.User,
                Email = user.Email,
                FechaNacimiento = user.FechaNacimiento
            };

            // Aplica el patch
            patchDoc.ApplyTo(userDto, error => ModelState.AddModelError(error.AffectedObject?.ToString() ?? "", error.ErrorMessage));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar cambio de nombre de usuario
            if (!string.IsNullOrWhiteSpace(userDto.Usuario) && !string.Equals(user.User, userDto.Usuario, StringComparison.OrdinalIgnoreCase))
            {
                var existe = await _context.User.AnyAsync(x => x.User == userDto.Usuario && x.Id != user.Id);
                if (existe)
                    return BadRequest(new { mensaje = "Ese nombre de usuario ya existe" });
                user.User = userDto.Usuario;
            }

            // Validar cambio de email
            if (!string.IsNullOrWhiteSpace(userDto.Email) && !string.Equals(user.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existeEmail = await _context.User.AnyAsync(x => x.Email == userDto.Email && x.Id != user.Id);
                if (existeEmail)
                    return BadRequest(new { mensaje = "Ese correo electrónico ya está en uso" });
                user.Email = userDto.Email;
            }
            user.FechaNacimiento = userDto.FechaNacimiento;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        /// <summary>
        /// Permite eliminar a un usuario por su id. Acceso solo para Administradores.
        /// </summary>
        // DELETE: api/users/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{usuario}")]
        public async Task<IActionResult> DeleteUser(string usuario)
        {
            if (string.IsNullOrEmpty(usuario))
                return BadRequest(new { mensaje = "Es requerido si o si el usuario" });

            var user = await _context.User.FirstOrDefaultAsync(x => x.User == usuario);
            if (user == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });


            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        /// <summary>
        /// Permite cambiar el rol de cualquier usuario. Acceso solo para Administradores.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{usuario}")]
        public async Task<IActionResult> UpdateRole([FromRoute] string usuario, [FromBody] UpdateRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(usuario))
                    return BadRequest(new { mensaje = "Es requerido si o si el usuario" });

                var user = await _context.User.FirstOrDefaultAsync(x => x.User == usuario);
                if (user == null)
                    return NotFound(new { mensaje = "Usuario no encontrado" });


                // UserRole es un enum, podrías validar si el valor es válido antes de asignar
                if (Enum.IsDefined(typeof(UserRole), dto.Role))
                    user.Role = dto.Role;


                await _context.SaveChangesAsync();
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }


    }

}

