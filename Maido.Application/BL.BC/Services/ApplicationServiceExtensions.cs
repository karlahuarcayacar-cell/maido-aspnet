using Microsoft.Extensions.DependencyInjection;

namespace Maido.Application.BL.BC.Services;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPlatilloService, PlatilloService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IReporteService, ReporteService>();
        
        return services;
    }
}
