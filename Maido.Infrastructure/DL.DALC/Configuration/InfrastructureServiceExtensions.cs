using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Maido.Infrastructure.DL.DALC.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Maido.Infrastructure.DL.DALC.Configuration;

/// <summary>
/// CAPA DE INFRAESTRUCTURA - CONFIGURACIÓN DE INYECCIÓN DE DEPENDENCIAS
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Registro de Repositorios (AddScoped):
///    Asocia la Interfaz de Dominio (ej: `IUsuarioRepository`) con su clase de implementación concreta de la Infraestructura (`UsuarioRepository`).
///    Cuando un servicio pida `IUsuarioRepository` en su constructor, la infraestructura le entregará automáticamente una instancia de `UsuarioRepository`.
/// 
/// 2. Registro de DbConnectionFactory (AddSingleton):
///    `AddSingleton` crea UNA sola instancia de `DbConnectionFactory` para toda la vida de la aplicación.
///    Es ideal para clases de fábrica que leen la cadena de conexión del archivo `appsettings.json`.
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Registrar la fábrica de conexiones SQL como Singleton
        services.AddSingleton<DbConnectionFactory>();

        // Registrar cada repositorio concreto asociado a su interfaz de dominio (Scoped)
        services.AddScoped<IPlatilloRepository, PlatilloRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IReporteRepository, ReporteRepository>();

        return services;
    }
}

