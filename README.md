# API para Venta de Entradas (En Desarrollo)

> **Estado actual:** Alpha (v1.0.0-alpha)  
> **Nota importante:** Este proyecto está en desarrollo y se va avanzando en mis ratos libres, ya que actualmente trabajo a tiempo completo. Por eso algunas funcionalidades aún no están terminadas.

---

## Índice

- [Descripción general](#descripción-general)  
- [Tecnologías usadas](#tecnologías-usadas)  
- [Estado actual y funcionalidades](#estado-actual-y-funcionalidades)  
- [Funcionalidades en desarrollo](#funcionalidades-en-desarrollo)  
- [Arquitectura y diseño artesanal](#arquitectura-y-diseño-artesanal)  
- [Instrucciones para clonar y correr](#instrucciones-para-clonar-y-correr)  
- [Roadmap y próximos pasos](#roadmap-y-próximos-pasos)  
- [Contribuciones](#contribuciones)  
- [Contacto y notas personales](#contacto-y-notas-personales)

---

## Descripción general

Esta API RESTful está pensada para gestionar la venta de entradas para eventos. Permite:

- Registro y autenticación de usuarios (sin usar Identity).  
- Gestión de eventos (crear, listar, actualizar, eliminar).  
- Compra de tickets y control de stock.  
- Integración con Redis para manejo de sesiones y tokens JWT.  
- Envío de confirmaciones con código QR y correo (parcialmente implementado).  
- Integración planeada con Mercado Pago para pagos.

---

## Tecnologías usadas

- .NET 9 y ASP.NET Core Web API  
- Entity Framework Core con SQLite (base de datos local)  
- Redis para cache y control de tokens JWT  
- JWT hecho artesanalmente (sin Identity)  
- QRCoder para generar códigos QR de entradas  
- MailKit para envío de emails  
- Mercado Pago SDK (planificado)  
- Swagger para documentación de API  

---

## Estado actual y funcionalidades

### ✅ Funcionalidades implementadas y utilizables

- **Registro de usuario**: con hash de contraseña hecho a mano.  
- **Login con JWT**: creación manual de token JWT y guardado en Redis para validar sesión única.  
- **Roles de usuario**: manejo básico de roles (Admin, Usuario).  
- **Listar y obtener usuarios**: sólo Admin puede listar y editar usuarios.  
- **Eventos**: listado básico de eventos y creación simple.  
- **Compra de tickets**: endpoint básico que permite registrar compras descontando stock.  
- **Swagger UI**: para probar los endpoints.  
- **Middleware de autenticación personalizado**: valida token contra Redis.  

---

## Funcionalidades en desarrollo 🚧

- **CRUD completo de eventos**: falta actualizar, eliminar, paginar.  
- **Sistema completo de compras**: pagos reales con Mercado Pago, generación y envío de QR por email.  
- **Gestión avanzada de roles y permisos**: incluir más roles y autorizaciones específicas.  
- **Sistema de baneos y verificación de usuarios**: Admin podrá banear o activar cuentas.  
- **Tests unitarios e integración**: para asegurar calidad del código.  
- **Documentación Postman**: colección para facilitar pruebas.  
- **Despliegue y CI/CD**: crear Dockerfile, pipeline para despliegue automático.  
- **Frontend React/React Native**: integración y consumo de esta API.  

---

## Arquitectura y diseño artesanal

Quiero hacer especial énfasis en que **no utilizo ASP.NET Identity ni ningún sistema prediseñado para autenticación**. Todo está hecho “a mano” para entender y controlar cada parte:

- **Hash y validación de contraseñas**: uso funciones hash seguras implementadas manualmente (PBKDF2/Bcrypt).  
- **JWT**: genero y valido tokens JWT directamente, sin librerías que hagan todo por mí.  
- **Redis**: guardo el token JWT activo para cada usuario para asegurar que sólo tenga una sesión válida a la vez. Esto evita problemas de tokens múltiples y mejora seguridad.  
- **Validación manual en middleware**: cada request protegido valida token contra Redis.  
- **Control completo de roles y permisos**: hecho en código sin librerías externas.

Esta forma de trabajo me ayuda a profundizar en la lógica detrás de la autenticación y la autorización, y evita dependencias pesadas o mágicas.

---

## Instrucciones para clonar y correr

### Requisitos previos

- .NET 9 SDK instalado  
- Redis instalado y corriendo en `localhost:6379` (configurable)  
- SQLite (no necesita instalación, se usa archivo local)  

### Pasos

1. Clonar el repositorio  
   ```bash
   git clone https://github.com/tu-usuario/tu-repo.git
   cd tu-repo
