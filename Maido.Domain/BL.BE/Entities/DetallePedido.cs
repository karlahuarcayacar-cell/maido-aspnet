namespace Maido.Domain.BL.BE.Entities;
public class DetallePedido
{
    public int IdDetalle { get; set; }
    public int IdPedido { get; set; }
    public int IdPlatillo { get; set; }
    public string NombrePlatillo { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal { get; set; }
}
