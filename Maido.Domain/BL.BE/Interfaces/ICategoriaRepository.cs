using Maido.Domain.BL.BE.Entities;

namespace Maido.Domain.BL.BE.Interfaces;

/// <summary>
/// CAPA DE DOMINIO - INTERFAZ DE REPOSITORIO: ICategoriaRepository
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Contrato de persistencia para la gestión de Categorías de platillos.
/// Separa la definición del comportamiento CRUD de la tecnología física de base de datos.
/// </summary>
public interface ICategoriaRepository
{
    /// <summary>
    /// Lista únicamente las categorías activas (Activo = true) para el catálogo público del cliente.
    /// </summary>
    Task<IEnumerable<Categoria>> ListarCategoriasAsync();

    /// <summary>
    /// Lista la totalidad de categorías (activas e inactivas) para la administración del panel de control.
    /// </summary>
    Task<IEnumerable<Categoria>> ListarCategoriasAdminAsync();

    /// <summary>
    /// Obtiene la entidad Categoria por su identificador primario (IdCategoria).
    /// </summary>
    Task<Categoria?> ObtenerCategoriaPorIdAsync(int idCategoria);

    /// <summary>
    /// Inserta una nueva categoría llamando a `sp_InsertarCategoria` y devuelve su Id generado.
    /// </summary>
    Task<int> InsertarCategoriaAsync(Categoria categoria);

    /// <summary>
    /// Actualiza los campos de una categoría existente.
    /// </summary>
    Task ActualizarCategoriaAsync(Categoria categoria);

    /// <summary>
    /// Realiza la eliminación (física o lógica según la regla de negocio) de la categoría especificada.
    /// </summary>
    Task EliminarCategoriaAsync(int idCategoria);
}

