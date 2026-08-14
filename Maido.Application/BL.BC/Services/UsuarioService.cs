using System.Security.Cryptography;
using System.Text;
using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - SERVICIO DE NEGOCIO: UsuarioService
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. Lógica de Negocio (Business Logic Layer):
///    Aquí residen las reglas de negocio de la aplicación (ej: verificar que las contraseñas coincidan, 
///    hashear claves antes de guardar en BD, validar que el usuario no esté deshabilitado).
/// 
/// 2. HASHING DE CONTRASEÑAS (MÉTODO CRÍTICO):
///    - ¿Por qué NO guardar la clave en texto plano?
///      Si la base de datos sufriera una fuga o vulnerabilidad, los atacantes obtendrían las contraseñas reales.
///    - Algoritmo utilizado: SHA-256 (Secure Hash Algorithm 256 bits).
///    - Hashing unidireccional: Una función Hash convierte "MiClave123" en una cadena hexadecimal fija de 64 caracteres.
///      No se puede "desencriptar" de vuelta a texto plano.
///    - Proceso de Autenticación: Cuando el usuario ingresa su contraseña en el Login, se aplica SHA-256 al texto ingresado 
///      y se compara el hash resultante con el hash almacenado previamente en SQL Server.
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    /// <summary>
    /// Constructor con Inyección de Dependencias.
    /// Recibe la interfaz `IUsuarioRepository`, lo que permite desacoplar el servicio de la base de datos concreta.
    /// </summary>
    public UsuarioService(IUsuarioRepository repo) => _repo = repo;

    /// <summary>
    /// MÉTODO CRÍTICO: Autenticación de Usuario.
    /// Pasos:
    /// 1. Busca el usuario en la BD por email.
    /// 2. Valida si existe y si su cuenta está activa.
    /// 3. Aplica SHA-256 a la contraseña enviada en el DTO de login.
    /// 4. Compara ambos hashes. Si coinciden, devuelve el UsuarioDto.
    /// </summary>
    public async Task<UsuarioDto?> AutenticarAsync(LoginDto dto)
    {
        var u = await _repo.ObtenerUsuarioPorEmailAsync(dto.Email);
        
        // Regla de Negocio: Si el usuario no existe o está deshabilitado por el administrador, denegar acceso.
        if (u is null || !u.Activo) return null;

        // Regla de Seguridad: Comparar Hash(Password Ingresada) == Hash Almacenado en BD
        if (u.PasswordHash != HashPassword(dto.Password)) return null;

        // Mapear la entidad de dominio Usuario hacia el DTO de transporte seguro
        return MapDto(u);
    }

    /// <summary>
    /// MÉTODO CRÍTICO: Registro de Nuevo Cliente.
    /// Pasos:
    /// 1. Valida que la contraseña y la confirmación sean idénticas.
    /// 2. Transforma la clave a su representación Hash (SHA-256).
    /// 3. Crea la Entidad Usuario asignándole el Rol por defecto (IdRol = 2 -> Cliente).
    /// 4. Invoca el repositorio para ejecutar el Stored Procedure `sp_RegistrarUsuario`.
    /// </summary>
    public async Task<(bool Exitoso, string Mensaje, int IdUsuario)> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        if (dto.Password != dto.ConfirmarPassword)
            return (false, "Las contraseñas no coinciden.", 0);

        var usuario = new Usuario
        {
            Nombre       = dto.Nombre,
            Apellido     = dto.Apellido,
            Email        = dto.Email,
            PasswordHash = HashPassword(dto.Password), // Cifrado unidireccional antes de persistir
            Telefono     = dto.Telefono,
            IdRol        = 2, // Rol de Cliente por defecto en el sistema
            Activo       = true,
            FechaRegistro = DateTime.Now
        };

        var id = await _repo.RegistrarUsuarioAsync(usuario);
        
        // El Stored Procedure sp_RegistrarUsuario devuelve -1 si el email ya existe en la tabla Usuarios.
        if (id == -1)
            return (false, "El correo electrónico ya está registrado.", 0);

        return (true, "Cuenta creada correctamente.", id);
    }

    /// <summary>
    /// Muestra la lista de usuarios del sistema mapeando cada Entidad a DTO.
    /// </summary>
    public async Task<IEnumerable<UsuarioDto>> ListarAsync()
    {
        var lista = await _repo.ListarUsuariosAsync();
        return lista.Select(MapDto);
    }

    /// <summary>
    /// Habilita o deshabilita la cuenta de un usuario en la base de datos.
    /// </summary>
    public async Task ActualizarEstadoAsync(int idUsuario, bool activo)
        => await _repo.ActualizarEstadoUsuarioAsync(idUsuario, activo);

    /// <summary>
    /// Obtiene los datos del perfil de un usuario para mostrarlos en el formulario de mi cuenta.
    /// </summary>
    public async Task<PerfilDto?> ObtenerPerfilPorEmailAsync(string email)
    {
        var u = await _repo.ObtenerUsuarioPorEmailAsync(email);
        if (u == null) return null;
        return new PerfilDto
        {
            IdUsuario = u.IdUsuario,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Email = u.Email,
            Telefono = u.Telefono,
            Direccion = u.Direccion
        };
    }

    /// <summary>
    /// Persiste los cambios de perfil realizados por el cliente en la base de datos.
    /// </summary>
    public async Task ActualizarPerfilAsync(PerfilDto dto)
    {
        var u = new Usuario
        {
            IdUsuario = dto.IdUsuario,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion
        };
        await _repo.ActualizarPerfilUsuarioAsync(u);
    }

    /// <summary>
    /// MÉTODO AUXILIAR CRÍTICO: HashPassword
    /// 
    /// Explicación Técnica para la Sustentación:
    /// 1. `SHA256.Create()`: Instancia el proveedor del algoritmo criptográfico estándar SHA-256.
    /// 2. `Encoding.UTF8.GetBytes(password)`: Convierte el string de la contraseña en una secuencia binaria de bytes en UTF-8.
    /// 3. `ComputeHash(...)`: Procesa el arreglo de bytes y produce un hash de 256 bits (32 bytes).
    /// 4. `BitConverter.ToString(...)`: Convierte la matriz de bytes resultante en una representación Hexadecimal limpia (64 caracteres).
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Helper estático de mapeo interno: Convierte una Entidad de Dominio [Usuario] a un [UsuarioDto].
    /// Evita repetir código de mapeo manual en múltiples métodos.
    /// </summary>
    private static UsuarioDto MapDto(Usuario u) => new()
    {
        IdUsuario     = u.IdUsuario,
        Nombre        = u.Nombre,
        Apellido      = u.Apellido,
        Email         = u.Email,
        Telefono      = u.Telefono,
        Direccion     = u.Direccion,
        IdRol         = u.IdRol,
        NombreRol     = u.NombreRol,
        Activo        = u.Activo,
        FechaRegistro = u.FechaRegistro
    };
}

