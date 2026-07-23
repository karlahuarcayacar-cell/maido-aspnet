namespace Maido.Application.BL.BC.DTOs;

public class CategoriaDto
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
}

public class CrearCategoriaDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
}

public class ActualizarCategoriaDto : CrearCategoriaDto
{
    public int IdCategoria { get; set; }
    public bool Activo { get; set; } = true;
}
