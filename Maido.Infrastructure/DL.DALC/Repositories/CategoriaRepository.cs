using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Microsoft.Data.SqlClient;

namespace Maido.Infrastructure.DL.DALC.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public CategoriaRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Categoria>> ListarCategoriasAsync()
    {
        var categorias = new List<Categoria>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarCategorias", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categorias.Add(MapCategoria(reader));
        }

        return categorias;
    }

    public async Task<IEnumerable<Categoria>> ListarCategoriasAdminAsync()
    {
        var categorias = new List<Categoria>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarCategoriasAdmin", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categorias.Add(MapCategoria(reader));
        }

        return categorias;
    }

    public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int idCategoria)
    {
        Categoria? categoria = null;
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ObtenerCategoriaPorId", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdCategoria", idCategoria);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            categoria = MapCategoria(reader);
        }

        return categoria;
    }

    public async Task<int> InsertarCategoriaAsync(Categoria categoria)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_InsertarCategoria", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
        command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
        command.Parameters.AddWithValue("@Icono", (object?)categoria.Icono ?? DBNull.Value);
        command.Parameters.AddWithValue("@Orden", categoria.Orden);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task ActualizarCategoriaAsync(Categoria categoria)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ActualizarCategoria", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@IdCategoria", categoria.IdCategoria);
        command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
        command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
        command.Parameters.AddWithValue("@Icono", (object?)categoria.Icono ?? DBNull.Value);
        command.Parameters.AddWithValue("@Orden", categoria.Orden);
        command.Parameters.AddWithValue("@Activo", categoria.Activo);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarCategoriaAsync(int idCategoria)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_EliminarCategoria", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdCategoria", idCategoria);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private Categoria MapCategoria(SqlDataReader reader)
    {
        return new Categoria
        {
            IdCategoria = reader.GetInt32(reader.GetOrdinal("IdCategoria")),
            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
            Icono = reader.IsDBNull(reader.GetOrdinal("Icono")) ? null : reader.GetString(reader.GetOrdinal("Icono")),
            Orden = reader.GetInt32(reader.GetOrdinal("Orden")),
            Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
        };
    }
}
