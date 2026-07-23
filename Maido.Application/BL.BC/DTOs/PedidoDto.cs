namespace Maido.Application.BL.BC.DTOs;

public class DetallePedidoDto
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => Precio * Cantidad;
}

public class CheckoutDto
{
    public string TipoPedido { get; set; } = "Delivery";
    public string? DireccionEntrega { get; set; }
    public string? Telefono { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public string? Observaciones { get; set; }
}

public class PedidoResumenDto
{
    public int IdPedido { get; set; }
    public DateTime FechaPedido { get; set; }
    public string TipoPedido { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
}

public class PedidoDetalleDto
{
    public int IdPedido { get; set; }
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
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public List<DetallePedidoDto> Items { get; set; } = [];
}

public class PedidosPaginadoDto
{
    public IEnumerable<PedidoResumenDto> Items { get; set; } = [];
    public int TotalRegistros { get; set; }
    public int PaginaActual { get; set; }
    public int RegistrosPorPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina);
}
