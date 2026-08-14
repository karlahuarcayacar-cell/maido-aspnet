namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// CAPA DE DOMINIO - ENTIDAD: DetallePedido
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Esta clase representa la línea del detalle en una orden de compra (Patrón Maestro-Detalle).
/// Mientras que [Pedido] guarda la cabecera (cliente, fecha, total), [DetallePedido] guarda CADA producto individual comprado, 
/// su cantidad y el subtotal calculado.
/// </summary>
public class DetallePedido
{
    /// <summary>
    /// Identificador único del registro de detalle en la base de datos.
    /// </summary>
    public int IdDetalle { get; set; }

    /// <summary>
    /// Clave foránea (FK) hacia la cabecera del pedido [Pedidos].
    /// </summary>
    public int IdPedido { get; set; }

    /// <summary>
    /// Clave foránea (FK) hacia el platillo adquirido.
    /// </summary>
    public int IdPlatillo { get; set; }

    /// <summary>
    /// Copia histórica del nombre del platillo al momento de realizar la compra.
    /// IMPORTANTE: Si el platillo cambia de nombre en el futuro, la boleta/pedido histórico mantiene el nombre original.
    /// </summary>
    public string NombrePlatillo { get; set; } = string.Empty;

    /// <summary>
    /// Copia histórica del precio unitario al momento de la compra.
    /// evita que cambios futuros de precios alteren contablemente ventas pasadas.
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Unidades compradas de este platillo.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Subtotal calculado de la línea: (PrecioUnitario * Cantidad).
    /// </summary>
    public decimal Subtotal { get; set; }
}

