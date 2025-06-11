# API-para-comprar-entradas

Una API RESTful construida con .NET 9 que simula un sistema de venta de entradas de eventos. Permite:

- Registro y login de usuarios con JWT y roles (Cliente, Organizador, Admin)  
- Gestión de usuarios (listar, ver, editar, eliminar, cambiar rol)  
- Gestión de roles (sólo lectura)  
- Control de un solo token activo por usuario (cache Redis)  
- Envío de tickets con QR via email  
- Módulo de eventos con CRUD y restricción de fechas/futuros  
- Migraciones y persistencia con Entity Framework + SQLite  

---

## 📦 Tecnologías y dependencias

- **.NET 9**  
- **Entity Framework Core** + SQLite  
- Autenticación y autorización:  
  - `Microsoft.AspNetCore.Authentication.JwtBearer`  
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`  
- Caching: `StackExchange.Redis` + `Microsoft.Extensions.Caching.StackExchangeRedis`  
- Documentación Swagger: `Swashbuckle.AspNetCore` + `Swashbuckle.AspNetCore.Filters`  
- Generación de QR: `QRCoder`  
- Envío de emails: `MailKit`  
- Mercado Pago SDK: `mercadopago-sdk`  
- JSON Patch: `Microsoft.AspNetCore.JsonPatch`  
- Newtonsoft JSON: `Microsoft.AspNetCore.Mvc.NewtonsoftJson`  

> Ver todas las referencias en el `<ItemGroup>` de tu `.csproj`.

---

## 🚀 Quick Start

1. **Clonar el repositorio**  
   ```bash
   git clone https://github.com/ExequielMontero/api-entradas.git
   cd api-entradas
