using Api_entradas.Data;
using Api_entradas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Api_entradas.Services
{
    public class AuthService
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _cfg;
        private readonly AppDbContext _db;

        public AuthService(IDistributedCache cache, IConfiguration cfg, AppDbContext db)
        {
            _cache = cache;
            _cfg = cfg;
            _db = db;
        }

        // 1. Hashear contraseña
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // 2. Registrar un nuevo usuario (rol Client por defecto)
        public async Task<User> RegisterAsync(string email, string password)
        {
            var hash = HashPassword(password);
            var user = new User
            {
                Email = email,
                PasswordHash = hash,
                Role = "Client"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        // 3. Validar credenciales y devolver User o null
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var hash = HashPassword(password);
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);
        }

        // 4. Generar JWT y cachear
        public async Task<string> GenerateTokenAsync(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email,       user.Email),
                new Claim(ClaimTypes.Role,        user.Role)
            };
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key),
                                               SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            await _cache.SetStringAsync(
                $"token:{user.Id}", jwt,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                });

            return jwt;
        }

        // 5. Validar token contra Redis
        public async Task<bool> ValidateCachedTokenAsync(Guid userId, string token)
        {
            var cached = await _cache.GetStringAsync($"token:{userId}");
            return cached == token;
        }
    }
}
