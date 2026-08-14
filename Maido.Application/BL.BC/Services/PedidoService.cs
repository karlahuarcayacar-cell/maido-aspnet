using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - SERVICIO DE NEGOCIO: PedidoService
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Lógica Financiera y de Negocio:
///    Este servicio calcula los montos monetarios oficiales de la transacción:
///    - Subtotal = Sumatoria de (Precio * Cantidad) de los ítems del carrito.
///    - IGV (18%) = Math.Round(Subtotal * 0.18m, 2).
///    - Total = Subtotal + IGV.
/// 
/// 2. Conversión de Carrito de Compras (Session) a Entidad de Dominio:
///    Toma los ítems seleccionados en la sesión web (`List<DetallePedidoDto>`) y las opciones de despacho (`CheckoutDto`), 
///    los transforma a la Entidad `Pedido` con su lista `Detalle`, y delega al Repositorio la ejecución del Stored Procedure transaccional.
/// </summary>
public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repo;

    /// <summary>
    /// Inyección de la interfaz del repositorio de pedidos.
    /// </summary>
    public PedidoService(IPedidoRepository repo) => _repo = repo;

    /// <summary>
    /// MÉTODO CRÍTICO: RegistrarPedidoAsync
    /// 
    /// Pasos:
    /// 1. Valida que el carrito no esté nulo o vacío.
    /// 2. Calcula Subtotal, IGV 18% y Total general de la orden.
    /// 3. Instancia la Entidad de Dominio `Pedido` con sus datos de envío y pago.
    /// 4. Mapea la lista de DTOs del carrito a la colección de Entidades `DetallePedido`.
    /// 5. Invoca a `_repo.RegistrarPedidoTransaccionalAsync(pedido)` el cual enviará un JSON a SQL Server.
    /// </summary>
    public async Task<int> RegistrarPedidoAsync(int idUsuario, List<DetallePedidoDto> carrito, CheckoutDto checkout)
    {
        if (carrito is null || !carrito.Any())
            throw new ArgumentException("El carrito no puede estar vacío.");

        // Cálculo de montos contables en C#
        decimal subtotal = carrito.Sum(x => x.Subtotal);
        decimal igv      = Math.Round(subtotal * 0.18m, 2); // 18% de Impuesto General a las Ventas
        decimal total    = subtotal + igv;

        var pedido = new Pedido
        {
            IdUsuario        = idUsuario,
            FechaPedido      = DateTime.Now,
            TipoPedido       = checkout.TipoPedido,
            DireccionEntrega = checkout.DireccionEntrega,
            Telefono         = checkout.Telefono,
            MetodoPago       = checkout.MetodoPago,
            Subtotal         = subtotal,
            IGV              = igv,
            Total            = total,
            Estado           = "Pendiente",
            Observaciones    = checkout.Observaciones,
            
            // Construcción del gráfico de objetos Maestro-Detalle
            Detalle = carrito.Select(item => new DetallePedido
            {
                IdPlatillo     = item.IdPlatillo,
                NombrePlatillo = item.Nombre,
                PrecioUnitario = item.Precio,
                Cantidad       = item.Cantidad,
                Subtotal       = item.Subtotal
            }).ToList()
        };

        // Delegación de la transacción al Repositorio
        return await _repo.RegistrarPedidoTransaccionalAsync(pedido);
    }

    /// <summary>
    /// Recupera la información completa de una orden efectuada (Cabecera + Ítems) para generar la boleta o confirmación.
    /// </summary>
    public async Task<PedidoDetalleDto?> ObtenerDetalleAsync(int idPedido)
    {
        var p = await _repo.ObtenerPedidoPorIdAsync(idPedido);
        if (p is null) return null;

        var detalles = await _repo.ObtenerDetallePedidoAsync(idPedido);

        return new PedidoDetalleDto
        {
            IdPedido         = p.IdPedido,
            NombreCliente    = p.NombreCliente,
            Email            = p.Email,
            FechaPedido      = p.FechaPedido,
            TipoPedido       = p.TipoPedido,
            DireccionEntrega = p.DireccionEntrega,
            Telefono         = p.Telefono,
            MetodoPago       = p.MetodoPago,
            Subtotal         = p.Subtotal,
            IGV              = p.IGV,
            Total            = p.Total,
            Estado           = p.Estado,
            Observaciones    = p.Observaciones,
            Items = detalles.Select(d => new DetallePedidoDto
            {
                IdPlatillo = d.IdPlatillo,
                Nombre     = d.NombrePlatillo,
                Precio     = d.PrecioUnitario,
                Cantidad   = d.Cantidad
            }).ToList()
        };
    }

    /// <summary>
    /// Obtiene el historial de pedidos de un cliente específico en la sección "Mis Pedidos".
    /// </summary>
    public async Task<IEnumerable<PedidoResumenDto>> ListarPorUsuarioAsync(int idUsuario)
    {
        var lista = await _repo.ListarPedidosPorUsuarioAsync(idUsuario);
        return lista.Select(MapResumen);
    }

    /// <summary>
    /// Consulta paginada para la bandeja administrativa de gestión de pedidos.
    /// </summary>
    public async Task<(IEnumerable<PedidoResumenDto> Items, int Total)> ListarPaginadoAsync(
        int pagina, int registrosPorPagina, string? estado, DateTime? fechaInicio, DateTime? fechaFin, int? idUsuario = null)
    {
        var (pedidos, total) = await _repo.ListarPedidosPaginadoAsync(pagina, registrosPorPagina, estado, fechaInicio, fechaFin, idUsuario);
        return (pedidos.Select(MapResumen), total);
    }

    /// <summary>
    /// Cambia el estado del pedido (ej: Pendiente -> En Preparación -> En Camino -> Entregado).
    /// </summary>
    public async Task ActualizarEstadoAsync(int idPedido, string estado)
        => await _repo.ActualizarEstadoPedidoAsync(idPedido, estado);

    /// <summary>
    /// Mapeador estático de Entidad Pedido a PedidoResumenDto.
    /// </summary>
    private static PedidoResumenDto MapResumen(Pedido p) => new()
    {
        IdPedido    = p.IdPedido,
        FechaPedido = p.FechaPedido,
        TipoPedido  = p.TipoPedido,
        MetodoPago  = p.MetodoPago,
        Total       = p.Total,
        Estado      = p.Estado,
        NombreCliente = p.NombreCliente
    };
}

