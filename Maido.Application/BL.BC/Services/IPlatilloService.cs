using Maido.Application.BL.BC.DTOs;

namespace Maido.Application.BL.BC.Services;

public interface IPlatilloService
{
    Task<IEnumerable<PlatilloDto>> ListarPublicoAsync(int? idCategoria, string? busqueda);
    Task<PlatillosPaginadoDto> ListarPaginadoAsync(int pagina, int registrosPorPagina, int? idCategoria, string? busqueda);
    Task<PlatilloDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearPlatilloDto dto);
    Task ActualizarAsync(ActualizarPlatilloDto dto);
    Task EliminarAsync(int id);
}
