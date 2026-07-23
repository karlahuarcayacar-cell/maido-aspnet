using Maido.Domain.BL.BE.Entities;
namespace Maido.Domain.BL.BE.Interfaces;
public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> ListarCategoriasAsync();
    Task<IEnumerable<Categoria>> ListarCategoriasAdminAsync();
    Task<Categoria?> ObtenerCategoriaPorIdAsync(int idCategoria);
    Task<int> InsertarCategoriaAsync(Categoria categoria);
    Task ActualizarCategoriaAsync(Categoria categoria);
    Task EliminarCategoriaAsync(int idCategoria);
}
