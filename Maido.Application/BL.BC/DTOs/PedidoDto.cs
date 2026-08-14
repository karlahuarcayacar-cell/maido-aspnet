namespace Maido.Application.BL.BC.DTOs;

/// <summary>
/// CAPA DE APLICACIÓN - DTOs DE PEDIDOS Y PROCESO DE CHECKOUT
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Estructuras de datos dedicadas a capturar los datos de la compra del carrito (CheckoutDto), 
/// presentar resúmenes paginados (PedidoResumenDto) y mostrar la boleta/confirmación detallada (PedidoDetalleDto).
/// </summary>

/// <summary>
/// DTO que representa cada producto contenido dentro del pedido.
/// Contiene una propiedad calculada [Subtotal].
/// </summary>
public class DetallePedidoDto
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => Precio * Cantidad;
}

/// <summary>
/// DTO capturado en la vista de Checkout con las opciones de entrega y pago.
/// </summary>
public class CheckoutDto
{
    public string TipoPedido { get; set; } = "Delivery";
    public string? DireccionEntrega { get; set; }
    public string? Telefono { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO ligero de cabecera utilizado para poblar las grillas o listas de pedidos (Mis Pedidos, Admin Pedidos).
/// </summary>
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

/// <summary>
/// DTO completo de lectura con la cabecera e historial detallado de ítems ([Items]).
/// </summary>
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

/// <summary>
/// DTO contenedor para respuestas paginadas de la lista de pedidos en el Panel Admin.
/// Incluye la propiedad calculada [TotalPaginas].
/// </summary>
public class PedidosPaginadoDto
{
    public IEnumerable<PedidoResumenDto> Items { get; set; } = [];
    public int TotalRegistros { get; set; }
    public int PaginaActual { get; set; }
    public int RegistrosPorPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina);
}

