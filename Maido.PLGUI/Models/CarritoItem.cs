namespace Maido.PLGUI.Models;

public class CarritoItem
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public string? ImagenUrl { get; set; }
    public decimal Subtotal => Precio * Cantidad;
}
