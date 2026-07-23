namespace Maido.Domain.BL.BE.Entities;
public class Pedido
{
    public int IdPedido { get; set; }
    public int IdUsuario { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaPedido { get; set; }
    public string TipoPedido { get; set; } = string.Empty;
    public string? DireccionEntrega { get; set; }
    public string? Telefono { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal IGV { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string? Observaciones { get; set; }
    public List<DetallePedido> Detalle { get; set; } = new();
}
