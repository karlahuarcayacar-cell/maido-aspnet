namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// CAPA DE DOMINIO - ENTIDAD: Platillo
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Representa cada ítem gastronómico en venta en la plataforma Maido.
/// Contiene sus atributos comerciales (precio, disponibilidad, categoría, imagen) y auditoría (fecha de alta).
/// </summary>
public class Platillo
{
    /// <summary>
    /// Identificador único (PK) del platillo en la base de datos.
    /// </summary>
    public int IdPlatillo { get; set; }

    /// <summary>
    /// Nombre comercial del platillo (ej: "Acevichado Roll 10 piezas").
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de los ingredientes o preparación.
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Precio de venta unitario expresado en soles (DECIMAL 10,2 en SQL Server).
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Ruta relativa o URL donde se encuentra alojada la imagen del platillo (/uploads/platillos/...).
    /// </summary>
    public string? ImagenUrl { get; set; }

    /// <summary>
    /// Clave foránea (FK) que vincula el platillo con su categoría correspondiente.
    /// </summary>
    public int IdCategoria { get; set; }

    /// <summary>
    /// Nombre de la categoría obtenido mediante INNER JOIN para simplificar la visualización en la UI.
    /// </summary>
    public string NombreCategoria { get; set; } = string.Empty;

    /// <summary>
    /// Bandera que controla el stock/disponibilidad ( true = En Stock, false = Agotado ).
    /// </summary>
    public bool Disponible { get; set; } = true;

    /// <summary>
    /// Indica si el platillo debe aparecer en la sección principal de Recomendaciones de la Home.
    /// </summary>
    public bool Destacado { get; set; } = false;

    /// <summary>
    /// Timestamp de la fecha en que se dio de alta el producto en el catálogo.
    /// </summary>
    public DateTime FechaAlta { get; set; }
}

