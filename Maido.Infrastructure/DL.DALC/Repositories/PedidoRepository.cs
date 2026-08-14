using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Microsoft.Data.SqlClient;

namespace Maido.Infrastructure.DL.DALC.Repositories;

/// <summary>
/// CAPA DE INFRAESTRUCTURA - REPOSITORIO CONCRETO: PedidoRepository
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE (CONCEPTO MÁS IMPORTANTE DEL PROYECTO):
/// 1. REGISTRO TRANSACCIONAL CON OPENJSON:
///    - En sistemas tradicionales, insertar la cabecera del pedido (`Pedidos`) y sus 5 ítems del detalle (`DetallePedido`) 
///      requería 6 viajes de ida y vuelta a la base de datos (6 Round-trips de red).
///    - En este proyecto se optimiza utilizando **JSON y Transacciones Atómicas en SQL Server**:
///      a) C# serializa la lista de detalles `pedido.Detalle` a un string JSON con `JsonSerializer.Serialize()`.
///      b) Se envía el JSON completo como parámetro `@DetalleJSON` al Stored Procedure `sp_RegistrarPedidoTransaccional`.
///      c) SQL Server inicia `BEGIN TRANSACTION`, inserta la cabecera, obtiene `SCOPE_IDENTITY()`, 
///         y usa `OPENJSON(@DetalleJSON)` para volcar masivamente los detalles en la tabla `DetallePedido`.
///      d) Si todo sale bien ejecuta `COMMIT TRANSACTION`. Si ocurre cualquier falla, ejecuta `ROLLBACK TRANSACTION`.
///    - Garantiza las propiedades **ACID**: Jamás quedará una cabecera grabada sin sus detalles ni viceversa.
/// 
/// 2. PARÁMETROS DE SALIDA (OUTPUT PARAMETERS):
///    Se utiliza `SqlParameter` con `Direction = ParameterDirection.Output` para que el Stored Procedure nos devuelva 
///    el `IdPedido` recién generado de manera inmediata.
/// </summary>
public class PedidoRepository : IPedidoRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    /// <summary>
    /// Inyección de la fábrica de conexiones SQL.
    /// </summary>
    public PedidoRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// MÉTODO CRÍTICO: RegistrarPedidoTransaccionalAsync
    /// Envía la cabecera y el array de ítems serializado a JSON para procesamiento atómico en SQL Server.
    /// </summary>
    public async Task<int> RegistrarPedidoTransaccionalAsync(Pedido pedido)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_RegistrarPedidoTransaccional", connection);
        command.CommandType = CommandType.StoredProcedure;

        // Serialización del gráfico de objetos C# a cadena JSON
        string detalleJson = JsonSerializer.Serialize(pedido.Detalle);

        command.Parameters.AddWithValue("@IdUsuario", pedido.IdUsuario);
        command.Parameters.AddWithValue("@TipoPedido", pedido.TipoPedido);
        command.Parameters.AddWithValue("@DireccionEntrega", (object?)pedido.DireccionEntrega ?? DBNull.Value);
        command.Parameters.AddWithValue("@Telefono", (object?)pedido.Telefono ?? DBNull.Value);
        command.Parameters.AddWithValue("@MetodoPago", pedido.MetodoPago);
        command.Parameters.AddWithValue("@Subtotal", pedido.Subtotal);
        command.Parameters.AddWithValue("@IGV", pedido.IGV);
        command.Parameters.AddWithValue("@Total", pedido.Total);
        command.Parameters.AddWithValue("@Observaciones", (object?)pedido.Observaciones ?? DBNull.Value);
        command.Parameters.AddWithValue("@DetalleJSON", detalleJson);

        // Definición del Parámetro de Salida OUTPUT para recuperar el ID autonumérico
        var outIdPedido = new SqlParameter("@IdPedido", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outIdPedido);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        // Extraer el valor retornado por el parámetro OUTPUT de SQL Server
        return (int)outIdPedido.Value;
    }

    /// <summary>
    /// Recupera la cabecera del pedido por ID llamando a `sp_ObtenerPedidoPorId`.
    /// </summary>
    public async Task<Pedido?> ObtenerPedidoPorIdAsync(int idPedido)
    {
        Pedido? pedido = null;
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ObtenerPedidoPorId", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdPedido", idPedido);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            pedido = MapPedido(reader);
        }

        return pedido;
    }

    /// <summary>
    /// Recupera la lista de productos comprados en un pedido llamando a `sp_ObtenerDetallePedido`.
    /// </summary>
    public async Task<IEnumerable<DetallePedido>> ObtenerDetallePedidoAsync(int idPedido)
    {
        var detalles = new List<DetallePedido>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ObtenerDetallePedido", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdPedido", idPedido);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            detalles.Add(MapDetallePedido(reader));
        }

        return detalles;
    }

    /// <summary>
    /// Obtiene las compras del cliente en "Mis Pedidos" invocando `sp_ListarPedidosPorUsuario`.
    /// </summary>
    public async Task<IEnumerable<Pedido>> ListarPedidosPorUsuarioAsync(int idUsuario)
    {
        var pedidos = new List<Pedido>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarPedidosPorUsuario", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdUsuario", idUsuario);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            pedidos.Add(MapPedido(reader));
        }

        return pedidos;
    }

    /// <summary>
    /// Consulta Paginada de Pedidos para la bandeja administrativa.
    /// Utiliza `SqlParameter` de salida `@TotalRegistros` para conocer cuántas páginas existen en total 
    /// aplicando OFFSET-FETCH NEXT en SQL Server.
    /// </summary>
    public async Task<(IEnumerable<Pedido> Pedidos, int TotalRegistros)> ListarPedidosPaginadoAsync(
        int pagina, int registrosPorPagina, string? estado, DateTime? fechaInicio, DateTime? fechaFin, int? idUsuario = null)
    {
        var pedidos = new List<Pedido>();
        int totalRegistros = 0;
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarPedidosPaginado", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Pagina", pagina);
        command.Parameters.AddWithValue("@RegistrosPorPagina", registrosPorPagina);
        command.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        command.Parameters.AddWithValue("@FechaInicio", (object?)fechaInicio ?? DBNull.Value);
        command.Parameters.AddWithValue("@FechaFin", (object?)fechaFin ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdUsuario", (object?)idUsuario ?? DBNull.Value);

        var outTotalRegistros = new SqlParameter("@TotalRegistros", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outTotalRegistros);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            pedidos.Add(MapPedido(reader));
        }

        await reader.CloseAsync();
        totalRegistros = (int)outTotalRegistros.Value;

        return (pedidos, totalRegistros);
    }

    /// <summary>
    /// Cambia el estado del pedido invocando a `sp_ActualizarEstadoPedido`.
    /// </summary>
    public async Task ActualizarEstadoPedidoAsync(int idPedido, string estado)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ActualizarEstadoPedido", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdPedido", idPedido);
        command.Parameters.AddWithValue("@Estado", estado);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Helper de mapeo defensivo para la cabecera de Pedido.
    /// </summary>
    private Pedido MapPedido(SqlDataReader reader)
    {
        var p = new Pedido
        {
            IdPedido   = reader.GetInt32(reader.GetOrdinal("IdPedido")),
            TipoPedido = reader.GetString(reader.GetOrdinal("TipoPedido")),
            MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
            Total      = reader.GetDecimal(reader.GetOrdinal("Total")),
            Estado     = reader.GetString(reader.GetOrdinal("Estado")),
            FechaPedido = reader.GetDateTime(reader.GetOrdinal("FechaPedido"))
        };
        if (ColumnExists(reader, "Subtotal"))
            p.Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal"));
        if (ColumnExists(reader, "IGV"))
            p.IGV = reader.GetDecimal(reader.GetOrdinal("IGV"));
        if (ColumnExists(reader, "IdUsuario"))
            p.IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario"));
        if (ColumnExists(reader, "DireccionEntrega") && !reader.IsDBNull(reader.GetOrdinal("DireccionEntrega")))
            p.DireccionEntrega = reader.GetString(reader.GetOrdinal("DireccionEntrega"));
        if (ColumnExists(reader, "Telefono") && !reader.IsDBNull(reader.GetOrdinal("Telefono")))
            p.Telefono = reader.GetString(reader.GetOrdinal("Telefono"));
        if (ColumnExists(reader, "Observaciones") && !reader.IsDBNull(reader.GetOrdinal("Observaciones")))
            p.Observaciones = reader.GetString(reader.GetOrdinal("Observaciones"));
        if (ColumnExists(reader, "NombreCliente") && !reader.IsDBNull(reader.GetOrdinal("NombreCliente")))
            p.NombreCliente = reader.GetString(reader.GetOrdinal("NombreCliente"));
        if (ColumnExists(reader, "Email") && !reader.IsDBNull(reader.GetOrdinal("Email")))
            p.Email = reader.GetString(reader.GetOrdinal("Email"));
        return p;
    }

    /// <summary>
    /// Helper de mapeo para las líneas del detalle de pedido.
    /// </summary>
    private DetallePedido MapDetallePedido(SqlDataReader reader)
    {
        var detalle = new DetallePedido
        {
            IdDetalle = reader.GetInt32(reader.GetOrdinal("IdDetalle")),
            IdPedido = reader.GetInt32(reader.GetOrdinal("IdPedido")),
            IdPlatillo = reader.GetInt32(reader.GetOrdinal("IdPlatillo")),
            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
            Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal"))
        };

        if (ColumnExists(reader, "NombrePlatillo") && !reader.IsDBNull(reader.GetOrdinal("NombrePlatillo")))
        {
            detalle.NombrePlatillo = reader.GetString(reader.GetOrdinal("NombrePlatillo"));
        }
        return detalle;
    }

    /// <summary>
    /// Comprueba la presencia de columnas en el SqlDataReader.
    /// </summary>
    private bool ColumnExists(SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}

