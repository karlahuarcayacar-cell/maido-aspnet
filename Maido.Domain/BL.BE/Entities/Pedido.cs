namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// CAPA DE DOMINIO - ENTIDAD: Pedido (Cabecera)
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Representa la cabecera de la orden transaccional efectuada por el cliente.
/// Mantiene los totales financieros (Subtotal, IGV 18%, Total), el flujo de estado (Pendiente, En Preparacion, En Camino, Entregado, Cancelado),
/// y la colección de ítems asociados en la propiedad de navegación [Detalle].
/// </summary>
public class Pedido
{
    /// <summary>
    /// Identificador único (PK autoincremental) del pedido.
    /// </summary>
    public int IdPedido { get; set; }

    /// <summary>
    /// Clave foránea (FK) del usuario/cliente que realizó la compra.
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nombre del cliente (obtenido mediante JOIN en las vistas administrativas).
    /// </summary>
    public string NombreCliente { get; set; } = string.Empty;

    /// <summary>
    /// Email del cliente para notificaciones o contacto.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora exacta de registro del pedido en el servidor.
    /// </summary>
    public DateTime FechaPedido { get; set; }

    /// <summary>
    /// Modalidad de entrega: "Delivery" o "Mesa" / "Para Llevar".
    /// </summary>
    public string TipoPedido { get; set; } = string.Empty;

    /// <summary>
    /// Dirección física consignada para el envío en caso de ser Delivery.
    /// </summary>
    public string? DireccionEntrega { get; set; }

    /// <summary>
    /// Teléfono de contacto de emergencia para el repartidor.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Forma de pago pactada: "Efectivo", "Tarjeta", "Yape/Plin".
    /// </summary>
    public string MetodoPago { get; set; } = string.Empty;

    /// <summary>
    /// Sumatoria de la base imponible sin impuestos.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Impuesto General a las Ventas (18% en Perú).
    /// </summary>
    public decimal IGV { get; set; }

    /// <summary>
    /// Monto final a cobrar (Subtotal + IGV).
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Estado del flujo del pedido: ("Pendiente", "En Preparacion", "En Camino", "Entregado", "Cancelado").
    /// </summary>
    public string Estado { get; set; } = "Pendiente";

    /// <summary>
    /// Comentarios o indicaciones especiales del cliente (ej: "Sin Wasabi", "Tocar timbre 201").
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Lista de ítems o líneas pertenecientes a este pedido (Relación 1 a Muchos).
    /// </summary>
    public List<DetallePedido> Detalle { get; set; } = new();
}

