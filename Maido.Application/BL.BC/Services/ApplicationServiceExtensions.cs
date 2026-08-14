using Microsoft.Extensions.DependencyInjection;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - EXTENSIÓN DE INYECCIÓN DE DEPENDENCIAS (IoC Container)
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es la Inyección de Dependencias (DI)?
///    Es un principio de diseño de software (D de SOLID) donde las clases reciben sus dependencias desde fuera
///    en lugar de crearlas directamente con `new`.
/// 
/// 2. Ciclo de Vida Scoped (AddScoped):
///    Se crea una única instancia del servicio por cada solicitud HTTP (HTTP Request).
///    Al terminar la solicitud del cliente, el contenedor elimina automáticamente la instancia y libera recursos.
/// 
/// 3. Método de Extensión:
///    El uso de `this IServiceCollection services` permite invocar `builder.Services.AddApplicationServices()`
///    en `Program.cs` manteniendo limpia la configuración principal del sistema.
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar servicios de la Capa de Aplicación con tiempo de vida Scoped
        services.AddScoped<IPlatilloService, PlatilloService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IReporteService, ReporteService>();
        
        return services;
    }
}

