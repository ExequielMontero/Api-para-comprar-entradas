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
using Microsoft.Extensions.FileProviders;
using Api_entradas.Utils;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);




// Cloudinary

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<CloudinaryService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<CloudinarySettings>>();
    var settings = options.Value;

    // TEMPORAL: imprime valores para verificar
    Console.WriteLine("CLOUDINARY CONFIG:");
    Console.WriteLine($"CloudName: {settings.CloudName}, ApiKey: {settings.ApiKey}, ApiSecret: {settings.ApiSecret}");

    return new CloudinaryService(settings);
});
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


// EF Core – SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis como IDistributedCache
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        new ConfigurationOptions
        {
            EndPoints = { "redis-12962.c93.us-east-1-3.ec2.redns.redis-cloud.com:12962" },
            User = "default",
            Password = "sC3qJHIwEp78By47uZ1dK07KGMXPOSK3"
        }
    )
);

// Servicios de la app
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<AuthService>();

// Configuración JWT
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
                ctx.Fail("Token inválido (en blacklist)");
            }
        }

    };
});

// Políticas de autorización
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

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});


// Pipeline
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1"));

app.UseCors(builder =>
    builder
        .WithOrigins("http://localhost:5173") // o el dominio de tu frontend
        .AllowAnyHeader()
        .AllowAnyMethod()
);
app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
