namespace Maido.Domain.BL.BE.Entities;

/// <summary>
/// CAPA DE DOMINIO - ENTIDAD: Usuario
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// 1. ¿Qué es una Entidad de Dominio?
///    Es la representación lógica pura de un concepto de negocio (en este caso, un Usuario del sistema).
///    No depende de bases de datos (SQL Server, MySQL, etc.) ni de tecnologías web (ASP.NET, Controllers, Views).
///    Es el núcleo de la Clean Architecture.
/// 
/// 2. Mapeo con la Base de Datos:
///    Cada propiedad de esta clase coincide con las columnas de la tabla [Usuarios] en SQL Server.
/// </summary>
public class Usuario
{
    /// <summary>
    /// Clave primaria (PK) del usuario en la base de datos (Identity / Autoincremental).
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nombre de pila del usuario (Obligatorio).
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del usuario (Obligatorio).
    /// </summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico único del usuario. Se usa como nombre de usuario para el inicio de sesión.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash criptográfico de la contraseña (SHA-256). 
    /// NUNCA se almacena la contraseña en texto plano por estándares de seguridad OWASP.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto opcional. (El '?' indica que admite valores NULL).
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Dirección física del cliente para despachos de pedidos (Opcional).
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Clave foránea (FK) hacia la tabla de Roles (1 = Administrador, 2 = Cliente).
    /// </summary>
    public int IdRol { get; set; }

    /// <summary>
    /// Nombre legible del rol cargado mediante un JOIN en la consulta SQL (ej. "Administrador", "Cliente").
    /// </summary>
    public string NombreRol { get; set; } = string.Empty;

    /// <summary>
    /// Estado lógico del usuario ( true = Activo / Habilitado, false = Inactivo / Bloqueado ).
    /// Permite borrado lógico sin destruir historial de ventas.
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Fecha y hora exacta en la que se registró el usuario en la plataforma.
    /// </summary>
    public DateTime FechaRegistro { get; set; }
}

