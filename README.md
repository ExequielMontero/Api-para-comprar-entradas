# API para Venta de Entradas (En Desarrollo)

> **Estado actual:** Alpha (v1.0.0-alpha)  
> **Nota importante:** Este proyecto está en desarrollo y se va avanzando en mis ratos libres, ya que actualmente trabajo y curso mis estudios en la facultad. Por eso algunas funcionalidades aún no están terminadas.

---

## Índice

- [Descripción general](#descripción-general)  
- [Tecnologías usadas por el momento](#tecnologías-usadas-por-el-momento)  
- [Funcionalidades implementadas y en desarrollo](#funcionalidades-implementadas-y-utilizables)  
- [Arquitectura y diseño artesanal](#arquitectura-y-diseño-artesanal)  
- [Instrucciones para clonar y correr](#instrucciones-para-clonar-y-correr)  
- [Contacto y notas personales](#contacto-y-notas-personales)

---

## Descripción general

Esta API RESTful está pensada para gestionar la venta de entradas para eventos. Permite:

- Registro y autenticación de usuarios con roles(sin usar Identity). (**TOTALMENTE FUNCIONAL**)
- Gestión de eventos (crear, listar, actualizar, eliminar) (**Controlador implementado pero esta EN DESARROLLO aún**).  
- Compra de tickets y control de stock. (**Controlador implementado pero esta EN DESARROLLO aún**).  
- Integración con Redis para manejo de sesiones y tokens JWT. (**TOTALMENTE FUNCIONAL**)
- Envío de confirmaciones con código QR y correo (parcialmente implementado). (**Funcionalidad en desarrollo**).
- Integración planeada con Mercado Pago para pagos. (**Funcionalidad en desarrollo**).
---

## Tecnologías usadas por el momento

- .NET 9 y ASP.NET Core Web API  
- Entity Framework Core con SQLite (base de datos local)  
- Redis para cache y control de tokens JWT  
- JWT hecho artesanalmente (sin Identity)  
- QRCoder para generar códigos QR de entradas  
- MailKit para envío de emails  
- Mercado Pago SDK (planificado)  
- Swagger para documentación de API  

---

### ✅ Funcionalidades implementadas y utilizables

## AuthController(endpoints)
- **Registro de usuario**: El registro funciona perfectamente y hashea la contraseña antes de insertarla en la db.  
- **Login con JWT**: Creación manual de token JWT y guardado en Redis para validar sesión única y que no se puedan crear muchos JWT y que que se puedan utilizar si no expiraron aún.  
- **Roles de usuario**: Manejo de roles (Admin, Cliente, Organizador).    

## AuthController(endpoints)
- **Lista obtener todos los usuarios**: Sólo Admin puede listar todos los usuarios.
- **Lista obtener usuarios por id**: Sólo Admin puede listar usuario por id.  
- **Actualizar usuario**: Cualquier usuario logueado puede actualizar cualquiera de sus datos exceptuando el rol. **Importante en el body quitar el atributo que no van a actualizar**.
- **Eliminar Usuario**: Sólo el Admin puede eliminar usuarios.  
- **Actualizar role**: Sólo el Admin puede actualizar el rol de cualquier usuario.  

## Funcionalidades en desarrollo 🚧

- **CRUD completo de eventos**: Esta bastante desarrollado pero hay que afinar ciertas cuestiones.
- **Sistema completo de compras**: Actualmente no funcional.
- **Sistema de baneos y verificación de usuarios**: Admin podrá banear o activar cuentas.  
- **Documentación Postman**: colección para facilitar pruebas (En desarrollo).  
- **Frontend React/React Native**: integración y consumo de esta API.
- **Despliegue y CI/CD**: crear Dockerfile, pipeline para despliegue automático (En esto no tengo casi nada de conocimiento pero estoy muy interesado en ver bien el tema y este proyecto me sirve para practicar e implementar estos temas. Ademas       actualmente estoy terminando de leer unos libros que me interesan sobre control de versiones y react y luego si me gustaria meterle de lleno a eso cuando se me liberen un poco los tiempos).
- **Tests unitarios e integración**: para asegurar calidad del código (Lo quiero hacer cuando ya considere que la api esta lista para ser lazada en la v1.0.0.  

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
- Redis esta corriendo en un servidor de prueba gratuito que provee la misma empresa de Redis asi que no tienen que configurar nada.  
- SQLite (no necesita instalación, se usa archivo local asi que no hay problema). 

### Pasos

1. Clonar el repositorio  
   ```bash
   git clone https://github.com/tu-usuario/tu-repo.git
   cd tu-repo
   
2. Abrir el proyecto en .net y correrlo.  
3. Se les abrira Swagger y ya podran empezar a utilizar los endpoints funcionales actualmente.
4. **Como nota importante**: Les recomiendo utilizar como primera vez el endpoint de login que ya tiene hardcodeado el correo y contraseña de un administrador, asi que directamente le tienen que dar a Execute y les devolvera el JWT que tienen que utilizar luego en el candadito que esta en cualquiera de los enpoint o en la parte superio del todo a la derecha de la pagina para insetar el JWT ahi y poder acceder a los recursos de los endpoints. Luego de eso les recomiendo que vayan a GetAllUsers y vean la lista completa de todos los usuarios con sus respectivos roles asi ya pueden empezar a experimentar con diferentes usuarios,etc o crear los suyos propios. 
