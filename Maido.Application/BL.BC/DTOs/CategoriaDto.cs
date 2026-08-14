namespace Maido.Application.BL.BC.DTOs;

/// <summary>
/// CAPA DE APLICACIÓN - DTOs DE CATEGORÍA
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Permiten transferir los datos de la entidad Categoria adaptándolos al flujo de creación, 
/// edición o listado en las vistas de administración y cliente.
/// </summary>

/// <summary>
/// DTO de lectura pública y administrativa de categorías.
/// </summary>
public class CategoriaDto
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
}

/// <summary>
/// DTO para la creación de una nueva categoría desde el formulario admin.
/// </summary>
public class CrearCategoriaDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
}

/// <summary>
/// DTO para la actualización de una categoría existente. Hereda de CrearCategoriaDto y añade IdCategoria y Activo.
/// </summary>
public class ActualizarCategoriaDto : CrearCategoriaDto
{
    public int IdCategoria { get; set; }
    public bool Activo { get; set; } = true;
}

