using Maido.Application.BL.BC.DTOs;

namespace Maido.Application.BL.BC.Services;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> ListarPublicasAsync();
    Task<IEnumerable<CategoriaDto>> ListarTodasAsync();
    Task<CategoriaDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearCategoriaDto dto);
    Task ActualizarAsync(ActualizarCategoriaDto dto);
    Task EliminarAsync(int id);
}
