using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

public class PlatilloService : IPlatilloService
{
    private readonly IPlatilloRepository _repo;

    public PlatilloService(IPlatilloRepository repo) => _repo = repo;

    public async Task<IEnumerable<PlatilloDto>> ListarPublicoAsync(int? idCategoria, string? busqueda)
    {
        var items = await _repo.ListarPlatillosPublicoAsync(idCategoria, busqueda);
        return items.Select(MapDto);
    }

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

    public async Task<PlatilloDto?> ObtenerPorIdAsync(int id)
    {
        var p = await _repo.ObtenerPlatilloPorIdAsync(id);
        return p is null ? null : MapDto(p);
    }

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

    public async Task EliminarAsync(int id)
        => await _repo.EliminarPlatilloAsync(id);

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
