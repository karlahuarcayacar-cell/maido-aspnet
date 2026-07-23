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

public class PedidoRepository : IPedidoRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PedidoRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> RegistrarPedidoTransaccionalAsync(Pedido pedido)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_RegistrarPedidoTransaccional", connection);
        command.CommandType = CommandType.StoredProcedure;

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

        var outIdPedido = new SqlParameter("@IdPedido", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outIdPedido);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return (int)outIdPedido.Value;
    }

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

    public async Task<(IEnumerable<Pedido> Pedidos, int TotalRegistros)> ListarPedidosPaginadoAsync(int pagina, int registrosPorPagina, string? estado, DateTime? fechaInicio, DateTime? fechaFin)
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
