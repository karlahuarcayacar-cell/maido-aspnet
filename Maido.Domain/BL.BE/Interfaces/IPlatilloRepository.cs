using Maido.Domain.BL.BE.Entities;

namespace Maido.Domain.BL.BE.Interfaces;

/// <summary>
/// CAPA DE DOMINIO - INTERFAZ DE REPOSITORIO: IPlatilloRepository
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Contrato de persistencia para el mantenimiento y consulta del catálogo de Platillos.
/// Soporta filtrados públicos, búsqueda por coincidencia textual, paginación y operaciones CRUD.
/// </summary>
public interface IPlatilloRepository
{
    /// <summary>
    /// Lista los platillos disponibles para los clientes con filtros opcionales de categoría y término de búsqueda.
    /// </summary>
    Task<IEnumerable<Platillo>> ListarPlatillosPublicoAsync(int? idCategoria, string? busqueda);

    /// <summary>
    /// Consulta paginada para el mantenimiento administrativo de platillos.
    /// </summary>
    Task<(IEnumerable<Platillo> Platillos, int TotalRegistros)> ListarPlatillosPaginadoAsync(
        int pagina, 
        int registrosPorPagina, 
        int? idCategoria, 
        string? busqueda);

    /// <summary>
    /// Busca un platillo por su identificador clave (IdPlatillo).
    /// </summary>
    Task<Platillo?> ObtenerPlatilloPorIdAsync(int idPlatillo);

    /// <summary>
    /// Inserta un nuevo platillo en la base de datos y retorna el ID asignado por el servidor SQL.
    /// </summary>
    Task<int> InsertarPlatilloAsync(Platillo platillo);

    /// <summary>
    /// Actualiza los atributos de un platillo existente.
    /// </summary>
    Task ActualizarPlatilloAsync(Platillo platillo);

    /// <summary>
    /// Elimina un platillo del sistema por su ID.
    /// </summary>
    Task EliminarPlatilloAsync(int idPlatillo);
}

