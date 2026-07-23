using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;
using Maido.Infrastructure.DL.DALC.Persistence;
using Microsoft.Data.SqlClient;

namespace Maido.Infrastructure.DL.DALC.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UsuarioRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Usuario?> ObtenerUsuarioPorEmailAsync(string email)
    {
        Usuario? usuario = null;
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ObtenerUsuarioPorEmail", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            usuario = MapUsuario(reader);
        }

        return usuario;
    }

    public async Task<int> RegistrarUsuarioAsync(Usuario usuario)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_RegistrarUsuario", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
        command.Parameters.AddWithValue("@Email", usuario.Email);
        command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
        command.Parameters.AddWithValue("@Telefono", (object?)usuario.Telefono ?? DBNull.Value);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<IEnumerable<Usuario>> ListarUsuariosAsync()
    {
        var usuarios = new List<Usuario>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ListarUsuarios", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            usuarios.Add(MapUsuario(reader));
        }

        return usuarios;
    }

    public async Task ActualizarEstadoUsuarioAsync(int idUsuario, bool activo)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ActualizarEstadoUsuario", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdUsuario", idUsuario);
        command.Parameters.AddWithValue("@Activo", activo);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private Usuario MapUsuario(SqlDataReader reader)
    {
        var usuario = new Usuario
        {
            IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
            IdRol = reader.GetInt32(reader.GetOrdinal("IdRol"))
        };

        if (ColumnExists(reader, "PasswordHash"))
            usuario.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

        // Try map NombreRol if present
        if (ColumnExists(reader, "NombreRol") && !reader.IsDBNull(reader.GetOrdinal("NombreRol")))
        {
            usuario.NombreRol = reader.GetString(reader.GetOrdinal("NombreRol"));
        }

        return usuario;
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
