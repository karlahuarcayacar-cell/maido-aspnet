using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Maido.Infrastructure.DL.DALC.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Maido.Infrastructure.DL.DALC.Configuration;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<DbConnectionFactory>();
        services.AddScoped<IPlatilloRepository, PlatilloRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IReporteRepository, ReporteRepository>();
        return services;
    }
}
