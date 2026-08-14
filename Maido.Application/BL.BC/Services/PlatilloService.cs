using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - SERVICIO DE NEGOCIO: PlatilloService
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Implementación de la lógica de catálogo de productos.
/// Traduce las solicitudes públicas (filtros de la carta por categoría o término de búsqueda) 
/// y administrativas (paginación, altas, bajas y modificaciones) hacia llamadas de repositorio.
/// </summary>
public class PlatilloService : IPlatilloService
{
    private readonly IPlatilloRepository _repo;

    public PlatilloService(IPlatilloRepository repo) => _repo = repo;

    /// <summary>
    /// Lista los platillos activos visibles en la carta web con filtros opcionales.
    /// </summary>
    public async Task<IEnumerable<PlatilloDto>> ListarPublicoAsync(int? idCategoria, string? busqueda)
    {
        var items = await _repo.ListarPlatillosPublicoAsync(idCategoria, busqueda);
        return items.Select(MapDto);
    }

    /// <summary>
    /// Genera la respuesta paginada con platillos y total de registros para los controles de paginación en las Vistas.
    /// </summary>
    public async Task<PlatillosPaginadoDto> ListarPaginadoAsync(int pagina, int registrosPorPagina, int? idCategoria, string? busqueda)
    {
        var (platillos, total) = await _repo.ListarPlatillosPaginadoAsync(pagina, registrosPorPagina, idCategoria, busqueda);
        return new PlatillosPaginadoDto
        {
            Items = platillos.Select(MapDto),
            TotalRegistros = total,
            PaginaActual = pagina,
            RegistrosPorPagina = registrosPorPagina
        };
    }

    /// <summary>
    /// Busca un platillo por ID y mapea la Entidad a DTO.
    /// </summary>
    public async Task<PlatilloDto?> ObtenerPorIdAsync(int id)
    {
        var p = await _repo.ObtenerPlatilloPorIdAsync(id);
        return p is null ? null : MapDto(p);
    }

    /// <summary>
    /// Mapea el DTO de creación a Entidad Platillo y delega su inserción al repositorio.
    /// </summary>
    public async Task<int> CrearAsync(CrearPlatilloDto dto)
    {
        var p = new Platillo
        {
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio      = dto.Precio,
            ImagenUrl   = dto.ImagenUrl,
            IdCategoria = dto.IdCategoria,
            Disponible  = dto.Disponible,
            Destacado   = dto.Destacado
        };
        return await _repo.InsertarPlatilloAsync(p);
    }

    /// <summary>
    /// Mapea el DTO de actualización a Entidad Platillo y ejecuta los cambios.
    /// </summary>
    public async Task ActualizarAsync(ActualizarPlatilloDto dto)
    {
        var p = new Platillo
        {
            IdPlatillo  = dto.IdPlatillo,
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio      = dto.Precio,
            ImagenUrl   = dto.ImagenUrl,
            IdCategoria = dto.IdCategoria,
            Disponible  = dto.Disponible,
            Destacado   = dto.Destacado
        };
        await _repo.ActualizarPlatilloAsync(p);
    }

    /// <summary>
    /// Elimina un platillo existente por su identificador primario.
    /// </summary>
    public async Task EliminarAsync(int id)
        => await _repo.EliminarPlatilloAsync(id);

    /// <summary>
    /// Helper estático de mapeo: Convierte Entidad Platillo -> PlatilloDto.
    /// </summary>
    private static PlatilloDto MapDto(Platillo p) => new()
    {
        IdPlatillo      = p.IdPlatillo,
        Nombre          = p.Nombre,
        Descripcion     = p.Descripcion,
        Precio          = p.Precio,
        ImagenUrl       = p.ImagenUrl,
        IdCategoria     = p.IdCategoria,
        NombreCategoria = p.NombreCategoria,
        Disponible      = p.Disponible,
        Destacado       = p.Destacado,
        FechaAlta       = p.FechaAlta
    };
}

