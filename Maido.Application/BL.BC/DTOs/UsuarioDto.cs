namespace Maido.Application.BL.BC.DTOs;

/// <summary>
/// CAPA DE APLICACIÓN - DTOs (Data Transfer Objects): UsuarioDto
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es un DTO (Objeto de Transferencia de Datos)?
///    Un DTO es un objeto plano (POCO) utilizado EXCLUSIVAMENTE para transportar datos entre capas 
///    (por ejemplo, de los Servicios a los Controladores MVC o la Vista).
/// 
/// 2. ¿Por qué usar DTOs en vez de Entidades directas?
///    - Seguridad: Evita exponer la contraseña hash (PasswordHash) o campos internos del Dominio a la Vista.
///    - Desacoplamiento: Si la tabla SQL cambia una columna, solo ajustamos el repositorio y el mapper; los DTOs protegen la vista.
///    - Optimización: Solo enviamos a la vista los campos que realmente necesita consumir.
/// </summary>

/// <summary>
/// DTO de lectura principal para la gestión de usuarios en la interfaz.
/// </summary>
public class UsuarioDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Propiedad calculada de solo lectura para presentar el nombre completo en vistas (ej: "Juan Pérez").
    /// </summary>
    public string NombreCompleto => $"{Nombre} {Apellido}";

    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}

/// <summary>
/// DTO utilizado para recibir las credenciales ingresadas en el formulario de Login.
/// </summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO utilizado para recepcionar los datos de registro de un nuevo cliente.
/// Incluye validación de confirmación de clave.
/// </summary>
public class RegistrarUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmarPassword { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

/// <summary>
/// DTO utilizado para la edición del perfil de usuario por parte del cliente logueado.
/// </summary>
public class PerfilDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty; 
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}

