using Maido.Domain.BL.BE.Entities;

namespace Maido.Domain.BL.BE.Interfaces;

/// <summary>
/// CAPA DE DOMINIO - INTERFAZ DE REPOSITORIO: IPedidoRepository
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Contrato encargado del almacenamiento transaccional de Pedidos.
/// Contiene operaciones de alta atomicidad (RegistrarPedidoTransaccionalAsync) y consultas complejas paginadas.
/// </summary>
public interface IPedidoRepository
{
    /// <summary>
    /// Registra un pedido y sus ítems de forma atómica/transaccional enviando un payload JSON al Stored Procedure.
    /// Garantiza las propiedades ACID (Atomicidad, Consistencia, Aislamiento, Durabilidad).
    /// </summary>
    Task<int> RegistrarPedidoTransaccionalAsync(Pedido pedido);

    /// <summary>
    /// Retorna un pedido específico por su ID junto a sus datos principales.
    /// </summary>
    Task<Pedido?> ObtenerPedidoPorIdAsync(int idPedido);

    /// <summary>
    /// Recupera la colección de ítems pertenecientes al detalle de un pedido en particular.
    /// </summary>
    Task<IEnumerable<DetallePedido>> ObtenerDetallePedidoAsync(int idPedido);

    /// <summary>
    /// Lista el historial de compras efectuadas por un cliente específico.
    /// </summary>
    Task<IEnumerable<Pedido>> ListarPedidosPorUsuarioAsync(int idUsuario);

    /// <summary>
    /// Consulta paginada de pedidos con filtros avanzados (estado, rango de fechas, usuario).
    /// Devuelve una tupla C# con la lista de pedidos filtrados y el total general de registros encontrados para la paginación.
    /// </summary>
    Task<(IEnumerable<Pedido> Pedidos, int TotalRegistros)> ListarPedidosPaginadoAsync(
        int pagina, 
        int registrosPorPagina, 
        string? estado, 
        DateTime? fechaInicio, 
        DateTime? fechaFin, 
        int? idUsuario = null);

    /// <summary>
    /// Cambia el estado operativo de un pedido ("Pendiente" -> "En Preparacion" -> "En Camino" -> "Entregado" / "Cancelado").
    /// </summary>
    Task ActualizarEstadoPedidoAsync(int idPedido, string estado);
}

