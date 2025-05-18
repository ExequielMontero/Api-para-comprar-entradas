using Api_entradas.Data;
using Api_entradas.Services;
using MercadoPago.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



Batteries.Init();
var builder = WebApplication.CreateBuilder(args);
MercadoPagoConfig.AccessToken = builder.Configuration["Mp:AccessToken"];
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core – SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();


/*Esto configura Redis como sistema de cache distribuido, usando la librería StackExchange.Redis.
-Se conecta a Redis en localhost:6379.
Prefija las claves almacenadas con "TicketAPI:" (útil si compartís el Redis con otras apps).
Puede ser usado para guardar tokens, sesiones, datos temporales, etc.*/

builder.Services.AddStackExchangeRedisCache(o => {
    o.Configuration = builder.Configuration["Redis:Configuration"];
    o.InstanceName = builder.Configuration["Redis:InstanceName"];
});


// Clave y parámetros JWT
var jwtSecret = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
          ValidateIssuer = false,
          ValidateAudience = false
      };
      options.Events = new JwtBearerEvents
      {
          OnTokenValidated = async ctx =>
          {
              var svc = ctx.HttpContext.RequestServices.GetRequiredService<AuthService>();
              var sub = ctx.Principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

              // Casteo seguro
              if (ctx.SecurityToken is JwtSecurityToken jwtToken)
              {
                  var ok = await svc.ValidateCachedTokenAsync(Guid.Parse(sub), jwtToken.RawData);
                  if (!ok)
                      ctx.Fail("Token no válido o caducado.");
              }
              else
              {
                  ctx.Fail("Token no es un JWT válido.");
              }
          }
      };
  });


/*Define políticas de autorización basadas en roles.
Ejemplo de uso en un controlador:
[Authorize(Policy = "Admin")]
[HttpPost("crear-evento")]*/
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Client", p => p.RequireRole("Client"));
    options.AddPolicy("Organizer", p => p.RequireRole("Organizer"));
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();

// Automigrations (opcional)
using (var scope = app.Services.CreateScope())
{
    AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
};


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
