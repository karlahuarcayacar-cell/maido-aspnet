using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Microsoft.Data.SqlClient;

namespace Maido.Infrastructure.DL.DALC.Repositories;

/// <summary>
/// CAPA DE INFRAESTRUCTURA - REPOSITORIO CONCRETO: PlatilloRepository
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Implementación con ADO.NET para las operaciones CRUD y consultas paginadas/filtradas 
/// de la tabla [Platillos] vinculada mediante JOIN con [Categorias].
/// </summary>
public class PlatilloRepository : IPlatilloRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PlatilloRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Consulta los platillos activos para la carta del cliente pasando parámetros opcionales `@IdCategoria` y `@Busqueda`.
    /// </summary>
    public async Task<IEnumerable<Platillo>> ListarPlatillosPublicoAsync(int? idCategoria, string? busqueda)
    {
        var platillos = new List<Platillo>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarPlatillosPublico", connection);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@IdCategoria", (object?)idCategoria ?? DBNull.Value);
        command.Parameters.AddWithValue("@Busqueda", (object?)busqueda ?? DBNull.Value);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            platillos.Add(MapPlatillo(reader));
        }

        return platillos;
    }

    /// <summary>
    /// Consulta Paginada de Platillos con OFFSET-FETCH NEXT y parámetro de salida `@TotalRegistros` (OUTPUT).
    /// </summary>
    public async Task<(IEnumerable<Platillo> Platillos, int TotalRegistros)> ListarPlatillosPaginadoAsync(int pagina, int registrosPorPagina, int? idCategoria, string? busqueda)
    {
        var platillos = new List<Platillo>();
        int totalRegistros = 0;
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarPlatillosPaginado", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Pagina", pagina);
        command.Parameters.AddWithValue("@RegistrosPorPagina", registrosPorPagina);
        command.Parameters.AddWithValue("@IdCategoria", (object?)idCategoria ?? DBNull.Value);
        command.Parameters.AddWithValue("@Busqueda", (object?)busqueda ?? DBNull.Value);

        var outTotalRegistros = new SqlParameter("@TotalRegistros", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outTotalRegistros);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            platillos.Add(MapPlatillo(reader));
        }

        await reader.CloseAsync();
        totalRegistros = (int)outTotalRegistros.Value;

        return (platillos, totalRegistros);
    }

    /// <summary>
    /// Busca un platillo específico por su clave primaria IdPlatillo mediante `sp_ObtenerPlatilloPorId`.
    /// </summary>
    public async Task<Platillo?> ObtenerPlatilloPorIdAsync(int idPlatillo)
    {
        Platillo? platillo = null;
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ObtenerPlatilloPorId", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdPlatillo", idPlatillo);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            platillo = MapPlatillo(reader);
        }

        return platillo;
    }

    /// <summary>
    /// Inserta un nuevo platillo con `sp_InsertarPlatillo` y retorna el ID autoincremental de SQL Server.
    /// </summary>
    public async Task<int> InsertarPlatilloAsync(Platillo platillo)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_InsertarPlatillo", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Nombre", platillo.Nombre);
        command.Parameters.AddWithValue("@Descripcion", platillo.Descripcion);
        command.Parameters.AddWithValue("@Precio", platillo.Precio);
        command.Parameters.AddWithValue("@ImagenUrl", (object?)platillo.ImagenUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdCategoria", platillo.IdCategoria);
        command.Parameters.AddWithValue("@Disponible", platillo.Disponible);
        command.Parameters.AddWithValue("@Destacado", platillo.Destacado);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    /// <summary>
    /// Modifica los atributos de un platillo con `sp_ActualizarPlatillo`.
    /// </summary>
    public async Task ActualizarPlatilloAsync(Platillo platillo)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ActualizarPlatillo", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@IdPlatillo", platillo.IdPlatillo);
        command.Parameters.AddWithValue("@Nombre", platillo.Nombre);
        command.Parameters.AddWithValue("@Descripcion", platillo.Descripcion);
        command.Parameters.AddWithValue("@Precio", platillo.Precio);
        command.Parameters.AddWithValue("@ImagenUrl", (object?)platillo.ImagenUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdCategoria", platillo.IdCategoria);
        command.Parameters.AddWithValue("@Disponible", platillo.Disponible);
        command.Parameters.AddWithValue("@Destacado", platillo.Destacado);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Elimina un platillo mediante `sp_EliminarPlatillo`.
    /// </summary>
    public async Task EliminarPlatilloAsync(int idPlatillo)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_EliminarPlatillo", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdPlatillo", idPlatillo);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Helper de Mapeo defensivo: Convierte la fila actual de `SqlDataReader` a la Entidad `Platillo`.
    /// </summary>
    private Platillo MapPlatillo(SqlDataReader reader)
    {
        var platillo = new Platillo
        {
            IdPlatillo = reader.GetInt32(reader.GetOrdinal("IdPlatillo")),
            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
            Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
            ImagenUrl = reader.IsDBNull(reader.GetOrdinal("ImagenUrl")) ? null : reader.GetString(reader.GetOrdinal("ImagenUrl")),
            IdCategoria = reader.GetInt32(reader.GetOrdinal("IdCategoria")),
            Disponible = reader.GetBoolean(reader.GetOrdinal("Disponible")),
            Destacado = reader.GetBoolean(reader.GetOrdinal("Destacado"))
        };

        if (ColumnExists(reader, "NombreCategoria") && !reader.IsDBNull(reader.GetOrdinal("NombreCategoria")))
            platillo.NombreCategoria = reader.GetString(reader.GetOrdinal("NombreCategoria"));

        if (ColumnExists(reader, "FechaAlta") && !reader.IsDBNull(reader.GetOrdinal("FechaAlta")))
            platillo.FechaAlta = reader.GetDateTime(reader.GetOrdinal("FechaAlta"));

        return platillo;
    }

    /// <summary>
    /// Comprueba la existencia de una columna específica por nombre en el SqlDataReader.
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

