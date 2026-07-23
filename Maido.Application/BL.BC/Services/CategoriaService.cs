using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;

    public CategoriaService(ICategoriaRepository repo) => _repo = repo;

    public async Task<IEnumerable<CategoriaDto>> ListarPublicasAsync()
    {
        var items = await _repo.ListarCategoriasAsync();
        return items.Select(MapDto);
    }

    public async Task<IEnumerable<CategoriaDto>> ListarTodasAsync()
    {
        var items = await _repo.ListarCategoriasAdminAsync();
        return items.Select(MapDto);
    }

    public async Task<CategoriaDto?> ObtenerPorIdAsync(int id)
    {
        var c = await _repo.ObtenerCategoriaPorIdAsync(id);
        return c is null ? null : MapDto(c);
    }

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

    public async Task EliminarAsync(int id)
        => await _repo.EliminarCategoriaAsync(id);

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
