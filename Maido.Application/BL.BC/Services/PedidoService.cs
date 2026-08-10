using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repo;

    public PedidoService(IPedidoRepository repo) => _repo = repo;

    public async Task<int> RegistrarPedidoAsync(int idUsuario, List<DetallePedidoDto> carrito, CheckoutDto checkout)
    {
        if (carrito is null || !carrito.Any())
            throw new ArgumentException("El carrito no puede estar vacío.");

        decimal subtotal = carrito.Sum(x => x.Subtotal);
        decimal igv      = Math.Round(subtotal * 0.18m, 2);
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
            Detalle = carrito.Select(item => new DetallePedido
            {
                IdPlatillo     = item.IdPlatillo,
                NombrePlatillo = item.Nombre,
                PrecioUnitario = item.Precio,
                Cantidad       = item.Cantidad,
                Subtotal       = item.Subtotal
            }).ToList()
        };

        return await _repo.RegistrarPedidoTransaccionalAsync(pedido);
    }

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

    public async Task<IEnumerable<PedidoResumenDto>> ListarPorUsuarioAsync(int idUsuario)
    {
        var lista = await _repo.ListarPedidosPorUsuarioAsync(idUsuario);
        return lista.Select(MapResumen);
    }

    public async Task<(IEnumerable<PedidoResumenDto> Items, int Total)> ListarPaginadoAsync(
        int pagina, int registrosPorPagina, string? estado, DateTime? fechaInicio, DateTime? fechaFin, int? idUsuario = null)
    {
        var (pedidos, total) = await _repo.ListarPedidosPaginadoAsync(pagina, registrosPorPagina, estado, fechaInicio, fechaFin, idUsuario);
        return (pedidos.Select(MapResumen), total);
    }

    public async Task ActualizarEstadoAsync(int idPedido, string estado)
        => await _repo.ActualizarEstadoPedidoAsync(idPedido, estado);

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
