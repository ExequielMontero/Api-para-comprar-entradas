using System.Security.Claims;
using Api_entradas.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_entradas.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.User.ToListAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }



        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [Authorize] // solo logueados
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // viene del JWT
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId != id.ToString() && currentUserRole != "Admin")
            {
                return Forbid(); // no puede editar a otro usuario
            }

            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            user.Email = dto.Email;
            user.FechaNacimiento = dto.FechaNacimiento;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId != id.ToString() && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

