namespace Maido.PLGUI.Models;

/// <summary>
/// CAPA DE PRESENTACIÓN - MODELO DE VISTA / MODELO TEMPORAL: CarritoItem
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Representa cada ítem individual almacenado temporalmente dentro de la Sesión Web del cliente.
/// Posee una propiedad calculada `Subtotal => Precio * Cantidad`.
/// </summary>
public class CarritoItem
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public string? ImagenUrl { get; set; }
    
    /// <summary>
    /// Subtotal del ítem en el carrito: (Precio Unitario * Cantidad de unidades).
    /// </summary>
    public decimal Subtotal => Precio * Cantidad;
}

