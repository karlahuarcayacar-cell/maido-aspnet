using Maido.Application.BL.BC.DTOs;

namespace Maido.Application.BL.BC.Services;

public interface IPedidoService
{
    Task<int> RegistrarPedidoAsync(int idUsuario, List<DetallePedidoDto> carrito, CheckoutDto checkout);
    Task<PedidoDetalleDto?> ObtenerDetalleAsync(int idPedido);
    Task<IEnumerable<PedidoResumenDto>> ListarPorUsuarioAsync(int idUsuario);
    Task<(IEnumerable<PedidoResumenDto> Items, int Total)> ListarPaginadoAsync(int pagina, int registrosPorPagina, string? estado, DateTime? fechaInicio, DateTime? fechaFin, int? idUsuario = null);
    Task ActualizarEstadoAsync(int idPedido, string estado);
}
