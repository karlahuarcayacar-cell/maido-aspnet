namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// CAPA DE DOMINIO - ENTIDAD: Categoria
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Representa la clasificación gastronómica de la carta del restaurante (Nigiris, Sashimi, Maki, Sopas, Postres, etc.).
/// Sirve para agrupar los platillos y facilitar el filtrado tanto en la interfaz pública como en el panel administrativo.
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador único (PK) de la categoría en SQL Server.
    /// </summary>
    public int IdCategoria { get; set; }

    /// <summary>
    /// Nombre visible de la categoría (ej: "Makis", "Entradas Top").
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional para dar contexto al usuario sobre los productos de esta sección.
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Nombre de la clase del icono CSS (ej: "fa-utensils", "fa-fish") utilizado en las vistas Razor.
    /// </summary>
    public string? Icono { get; set; }

    /// <summary>
    /// Valor entero para definir la secuencia u orden de despliegue en la barra de menú o carta.
    /// </summary>
    public int Orden { get; set; }

    /// <summary>
    /// Indica si la categoría está activa en la carta pública. 
    /// Si es false, sus platillos asociados no se mostrarán en la tienda.
    /// </summary>
    public bool Activo { get; set; } = true;
}

