namespace Maido.Application.BL.BC.DTOs;

public class PlatilloDto
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int IdCategoria { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public bool Disponible { get; set; }
    public bool Destacado { get; set; }
    public DateTime FechaAlta { get; set; }
}

public class CrearPlatilloDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int IdCategoria { get; set; }
    public bool Disponible { get; set; } = true;
    public bool Destacado { get; set; } = false;
}

public class ActualizarPlatilloDto : CrearPlatilloDto
{
    public int IdPlatillo { get; set; }
}

public class PlatillosPaginadoDto
{
    public IEnumerable<PlatilloDto> Items { get; set; } = [];
    public int TotalRegistros { get; set; }
    public int PaginaActual { get; set; }
    public int RegistrosPorPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina);
}
