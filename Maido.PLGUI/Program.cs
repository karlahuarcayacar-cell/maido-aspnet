using Maido.Application.BL.BC.Services;
using Maido.Infrastructure.DL.DALC.Configuration;

/// <summary>
/// CAPA DE PRESENTACIÓN (PLGUI) - PUNTO DE ENTRADA PRINCIPAL: Program.cs
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es Program.cs en .NET Core / .NET 8+ / .NET 10?
///    Es el archivo de arranque (Bootstrapper) donde se configura el servidor web ASP.NET Core, 
///    se registran todos los servicios en el Contenedor de Inyección de Dependencias (IoC)
///    y se construye la Tubería de Procesamiento de Solicitudes (Middleware Pipeline).
/// 
/// 2. Inyección de Dependencias entre Capas:
///    Invocamos los métodos de extensión `AddInfrastructureServices()` y `AddApplicationServices()` 
///    para ensamblar las 4 capas de la Clean Architecture en el proyecto web.
/// 
/// 3. Configuración de Sesiones HTTP (`ISession` / Cookies):
///    Configura la memoria distribuida en RAM (`AddDistributedMemoryCache`) y las cookies de sesión
///    con atibutos de seguridad `HttpOnly = true` para prevenir ataques XSS (Cross-Site Scripting).
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS EN EL CONTENEDOR IoC (builder.Services)

// Habilita el patrón Modelo-Vista-Controlador (MVC) en la aplicación
builder.Services.AddControllersWithViews();

// Habilita el almacenamiento de variables de sesión en memoria RAM
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Expiración automática por inactividad tras 60 minutos
    options.Cookie.HttpOnly = true;                  // Protege la cookie impidiendo su lectura por Javascript del lado del cliente
    options.Cookie.IsEssential = true;              // Permite la cookie aunque el navegador tenga restricciones estrictas
    options.Cookie.Name = ".Maido.Session";         // Nombre personalizado de la cookie encrypted
});

// Registrar las capas de Infraestructura y Aplicación mediante Métodos de Extensión
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

// Licencia Comunitaria Gratuita de QuestPDF para exportación de reportes ejecutivos en PDF
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Permite acceder al contexto HTTP (Session, User, Cookies) desde clases Helper fuera de Controllers
builder.Services.AddHttpContextAccessor();

// Construcción del host web
var app = builder.Build();

// 2. CONFIGURACIÓN DEL MIDDLEWARE PIPELINE (app.Use...)
// El orden en que se agregan los middlewares determina el flujo de ejecución de cada petición HTTP.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirige peticiones HTTP inseguras hacia HTTPS
app.UseStaticFiles();       // Permite servir archivos estáticos (wwwroot: CSS, JS, imágenes de platillos)
app.UseRouting();          // Habilita la coincidencia de URLs con los Controllers (Routing)

app.UseSession();          // IMPORTANTE: Habilita el middleware de Sesión ANTES de la Autorización

app.UseAuthorization();    // Procesa los permisos y roles de acceso

// 3. MAPEO DE RUTAS (Routing Maps)

// Ruta personalizada para la administración (ej: /Admin/Dashboard, /Admin/Platillos)
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Dashboard}/{id?}",
    defaults: new { controller = "Admin" });

// Ruta por defecto del sitio web público (ej: /Home/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Iniciar el servidor web Kestrel y escuchar peticiones
app.Run();

