namespace Maido.Application.BL.BC.DTOs;

/// <summary>
/// CAPA DE APLICACIÓN - DTOs DE PLATILLOS
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Objetos DTO para transferir datos de productos/platillos entre la vista y los servicios de aplicación.
/// </summary>

/// <summary>
/// DTO de lectura principal de platillos.
/// </summary>
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

/// <summary>
/// DTO para la creación de un nuevo platillo desde la vista administrativa.
/// </summary>
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

/// <summary>
/// DTO para la edición de platillos existentes. Hereda de CrearPlatilloDto y agrega el IdPlatillo.
/// </summary>
public class ActualizarPlatilloDto : CrearPlatilloDto
{
    public int IdPlatillo { get; set; }
}

/// <summary>
/// DTO contenedor para enviar resultados paginados de la carta a las vistas.
/// </summary>
public class PlatillosPaginadoDto
{
    public IEnumerable<PlatilloDto> Items { get; set; } = [];
    public int TotalRegistros { get; set; }
    public int PaginaActual { get; set; }
    public int RegistrosPorPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina);
}

