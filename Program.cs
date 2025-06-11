using Api_entradas.Data;
using Api_entradas.Services;
using MercadoPago.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
var builder = WebApplication.CreateBuilder(args);

// Configurar MercadoPago
MercadoPagoConfig.AccessToken = builder.Configuration["Mp:AccessToken"];

// Configurar Swagger con JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ExampleFilters();

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

//registrar ejemplos
builder.Services.AddSwaggerExamplesFromAssemblyOf<PatchUserRequestExample>();

//Json
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Si usas Program.cs


// EF Core � SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis como IDistributedCache
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        new ConfigurationOptions
        {
            EndPoints = { "redis-19939.c251.east-us-mz.azure.redns.redis-cloud.com:19939" },
            User = "default",
            Password = "YgjSOkn8SDeI3XJODnf83kxIj8cRYNBb"
        }
    )
);

// Servicios de la app
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<AuthService>();

// Configuraci�n JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"]!;
var issuer = jwtSettings["Issuer"]!;
var audience = jwtSettings["Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            var redisDb = ctx.HttpContext.RequestServices
                .GetRequiredService<IConnectionMultiplexer>()
                .GetDatabase();

            var token = ctx.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var key = $"invalid:{token}";
            var isInvalid = await redisDb.StringGetAsync(key);

            Console.WriteLine($"[DEBUG] Revisando clave: {key} - Valor: {isInvalid}");

            if (!isInvalid.IsNullOrEmpty)
            {
                ctx.Fail("Token inv�lido (en blacklist)");
            }
        }

    };
});

// Pol�ticas de autorizaci�n
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Cliente", p => p.RequireRole("Cliente"));
    options.AddPolicy("Organizador", p => p.RequireRole("Organizador"));
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
});

var app = builder.Build();



app.MapGet("/hashpassword/{password}", async (string password) =>
{
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);

});

// Pipeline
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1"));

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
