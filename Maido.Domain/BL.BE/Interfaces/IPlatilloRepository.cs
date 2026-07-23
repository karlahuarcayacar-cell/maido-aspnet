using Maido.Domain.BL.BE.Entities;
namespace Maido.Domain.BL.BE.Interfaces;
public interface IPlatilloRepository
{
    Task<IEnumerable<Platillo>> ListarPlatillosPublicoAsync(int? idCategoria, string? busqueda);
    Task<(IEnumerable<Platillo> Platillos, int TotalRegistros)> ListarPlatillosPaginadoAsync(int pagina, int registrosPorPagina, int? idCategoria, string? busqueda);
    Task<Platillo?> ObtenerPlatilloPorIdAsync(int idPlatillo);
    Task<int> InsertarPlatilloAsync(Platillo platillo);
    Task ActualizarPlatilloAsync(Platillo platillo);
    Task EliminarPlatilloAsync(int idPlatillo);
}
