using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api_entradas.Data;
using Api_entradas.Enums;
using Api_entradas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using static QRCoder.PayloadGenerator;

namespace Api_entradas.Services
{
    public class AuthService
    {
        private readonly IDatabase _redisDb;
        private readonly IConfiguration _cfg;
        private readonly IEmailService _email;
        private readonly AppDbContext _db;

        public AuthService(IConnectionMultiplexer mux, IConfiguration cfg, AppDbContext db, IEmailService email)
        {
            _redisDb = mux.GetDatabase();
            _cfg = cfg;
            _email = email;
            _db = db;
        }

        //Metodo para hashear contraseña
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // 2. Registrar un nuevo usuario (rol Client por defecto)
        //dto.User,dto.Email, dto.Password,dto.FechaNacimiento,dto.Role
        public async Task<User> RegisterAsync(string usu, string email, string password, DateTime fechanacimiento)
        {
            if (_db.User.Any(u => u.Email == email))
            {
                throw new Exception("El email ya está registrado.");
            }
            else if (_db.User.Any(u => u.Usuario == usu))
            {
                throw new Exception("El usuario ya está registrado.");
            }

            var hash = HashPassword(password);
            var usuario = new User
            {
                Usuario = usu,
                Email = email,
                PasswordHash = hash,
                FechaNacimiento = fechanacimiento,
                Role = UserRole.Cliente
            };
            _db.User.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;


        }

        // 3. Validar credenciales y devolver User o null
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var hash = HashPassword(password);
            return await _db.User
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);
        }

        // 4. Generar JWT y cachear
        public async Task<string> GenerateTokenAsync(User user)
        {
            var jwtKey = _cfg["Jwt:Key"];
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Usuario),
                new Claim(ClaimTypes.Role, user.Role.ToString())};
            var key = Encoding.UTF8.GetBytes(jwtKey!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key),
                                               SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var oldToken = await _redisDb.StringGetAsync($"token:{user.Id}");
            Console.WriteLine($"[DEBUG] Valor en Redis para invalid: {oldToken}");

            if (!string.IsNullOrEmpty(oldToken))
            {
                await _redisDb.StringSetAsync($"invalid:{oldToken}", "true", TimeSpan.FromMinutes(60));
                await _redisDb.KeyDeleteAsync($"token:{user.Id}");
            }

            await _redisDb.StringSetAsync($"token:{user.Id}", jwt, TimeSpan.FromMinutes(60));

            return jwt;
        }

        //// 5. Validar token contra Redis
        //public async Task<bool> ValidateCachedTokenAsync(Guid userId, string token)
        //{
        //    // Verifica si el token está en la lista negra
        //    var invalid = await _cache.GetStringAsync($"invalid:{token}");
        //    Console.WriteLine($"[DEBUG] Valor en Redis para invalid: {invalid}");
        //    if (invalid != null) return false;
        //    var cached = await _cache.GetStringAsync($"token:{userId}");
        //    Console.WriteLine($"[DEBUG] Valor en Redis para invalid: {cached}");
        //    return cached == token;
        //}
    }

}
