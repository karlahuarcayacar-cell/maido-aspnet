using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - SERVICIO DE NEGOCIO: CategoriaService
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Servicio encargado de administrar las categorías del menú de Maido.
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;

    public CategoriaService(ICategoriaRepository repo) => _repo = repo;

    /// <summary>
    /// Lista únicamente las categorías marcadas como activas para la carta pública.
    /// </summary>
    public async Task<IEnumerable<CategoriaDto>> ListarPublicasAsync()
    {
        var items = await _repo.ListarCategoriasAsync();
        return items.Select(MapDto);
    }

    /// <summary>
    /// Lista la totalidad de categorías para la grilla de administración.
    /// </summary>
    public async Task<IEnumerable<CategoriaDto>> ListarTodasAsync()
    {
        var items = await _repo.ListarCategoriasAdminAsync();
        return items.Select(MapDto);
    }

    /// <summary>
    /// Obtiene una categoría por su ID.
    /// </summary>
    public async Task<CategoriaDto?> ObtenerPorIdAsync(int id)
    {
        var c = await _repo.ObtenerCategoriaPorIdAsync(id);
        return c is null ? null : MapDto(c);
    }

    /// <summary>
    /// Crea una nueva categoría asignándole estado activo por defecto.
    /// </summary>
    public async Task<int> CrearAsync(CrearCategoriaDto dto)
    {
        var c = new Categoria
        {
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion,
            Icono       = dto.Icono,
            Orden       = dto.Orden,
            Activo      = true
        };
        return await _repo.InsertarCategoriaAsync(c);
    }

    /// <summary>
    /// Actualiza los atributos de una categoría.
    /// </summary>
    public async Task ActualizarAsync(ActualizarCategoriaDto dto)
    {
        var c = new Categoria
        {
            IdCategoria = dto.IdCategoria,
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion,
            Icono       = dto.Icono,
            Orden       = dto.Orden,
            Activo      = dto.Activo
        };
        await _repo.ActualizarCategoriaAsync(c);
    }

    /// <summary>
    /// Elimina una categoría del sistema.
    /// </summary>
    public async Task EliminarAsync(int id)
        => await _repo.EliminarCategoriaAsync(id);

    /// <summary>
    /// Mapeador estático Entidad Categoria -> CategoriaDto.
    /// </summary>
    private static CategoriaDto MapDto(Categoria c) => new()
    {
        IdCategoria = c.IdCategoria,
        Nombre      = c.Nombre,
        Descripcion = c.Descripcion,
        Icono       = c.Icono,
        Orden       = c.Orden,
        Activo      = c.Activo
    };
}

