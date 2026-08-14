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
/// CAPA DE INFRAESTRUCTURA - REPOSITORIO CONCRETO: UsuarioRepository
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ADO.NET Purista con Stored Procedures:
///    Se utiliza el proveedor de datos de SQL Server (`Microsoft.Data.SqlClient`) mediante:
///    - `SqlConnection`: Administra la conexión física a la BD SQL Server.
///    - `SqlCommand`: Configura y ejecuta Procedimientos Almacenados (Stored Procedures).
///    - `SqlDataReader`: Lee los registros fila por fila desde la base de datos de manera altamente eficiente.
/// 
/// 2. Programación Asíncrona (`async` / `await` / `Task`):
///    - Métodos como `OpenAsync()`, `ExecuteReaderAsync()`, `ExecuteScalarAsync()`, `ExecuteNonQueryAsync()` y `ReadAsync()`
///      liberan el hilo del servidor web (ThreadPool thread) mientras SQL Server procesa la consulta.
///    - Esto permite que el servidor web soporte miles de usuarios concurrentes sin congelar recursos de CPU.
/// 
/// 3. Bloques `using var`:
///    Utiliza la declaración `using` de C# 8+. Garantiza que las conexiones y comandos SQL se cierren y destruyan (Dispose)
///    inmediatamente al terminar el método, evitando fugas de memoria o conexiones colgadas en SQL Server.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    /// <summary>
    /// Recibe la fábrica de conexiones mediante Inyección de Dependencias.
    /// </summary>
    public UsuarioRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Invoca al SP `sp_ObtenerUsuarioPorEmail` para recuperar los datos del usuario.
    /// </summary>
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

    /// <summary>
    /// Invoca al SP `sp_RegistrarUsuario`. 
    /// Utiliza `ExecuteScalarAsync()` porque el Stored Procedure retorna el ID generado (SCOPE_IDENTITY()) o -1 en caso de duplicidad.
    /// </summary>
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

    /// <summary>
    /// Invoca al SP `sp_ListarUsuarios` devolviendo la lista completa.
    /// </summary>
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

    /// <summary>
    /// Invoca al SP `sp_ActualizarEstadoUsuario`. Utiliza `ExecuteNonQueryAsync()` ya que no retorna filas.
    /// </summary>
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

    /// <summary>
    /// Invoca al SP `sp_ActualizarPerfilUsuario`.
    /// </summary>
    public async Task ActualizarPerfilUsuarioAsync(Usuario usuario)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("sp_ActualizarPerfilUsuario", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
        command.Parameters.AddWithValue("@Telefono", (object?)usuario.Telefono ?? DBNull.Value);
        command.Parameters.AddWithValue("@Direccion", (object?)usuario.Direccion ?? DBNull.Value);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// HELPER DE MAPEO MANUAL (SqlDataReader -> Usuario Entity):
    /// Extrae el valor de cada columna utilizando ordinales dinámicos (`reader.GetOrdinal("Columna")`).
    /// Verifica nulabilidad con `reader.IsDBNull` para evitar excepciones de casteo `NullReferenceException`.
    /// </summary>
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

        if (ColumnExists(reader, "Direccion") && !reader.IsDBNull(reader.GetOrdinal("Direccion")))
            usuario.Direccion = reader.GetString(reader.GetOrdinal("Direccion"));

        if (ColumnExists(reader, "FechaRegistro") && !reader.IsDBNull(reader.GetOrdinal("FechaRegistro")))
            usuario.FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"));

        if (ColumnExists(reader, "PasswordHash"))
            usuario.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

        if (ColumnExists(reader, "NombreRol") && !reader.IsDBNull(reader.GetOrdinal("NombreRol")))
        {
            usuario.NombreRol = reader.GetString(reader.GetOrdinal("NombreRol"));
        }

        return usuario;
    }

    /// <summary>
    /// Comprueba de manera defensiva si una columna existe en el resultado de la consulta actual.
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

