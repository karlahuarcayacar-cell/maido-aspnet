using Maido.Domain.BL.BE.Entities;
namespace Maido.Domain.BL.BE.Interfaces;
public interface IPedidoRepository
{
    Task<int> RegistrarPedidoTransaccionalAsync(Pedido pedido);
    Task<Pedido?> ObtenerPedidoPorIdAsync(int idPedido);
    Task<IEnumerable<DetallePedido>> ObtenerDetallePedidoAsync(int idPedido);
    Task<IEnumerable<Pedido>> ListarPedidosPorUsuarioAsync(int idUsuario);
    Task<(IEnumerable<Pedido> Pedidos, int TotalRegistros)> ListarPedidosPaginadoAsync(int pagina, int registrosPorPagina, string? estado, DateTime? fechaInicio, DateTime? fechaFin, int? idUsuario = null);
    Task ActualizarEstadoPedidoAsync(int idPedido, string estado);
}
