using Microsoft.AspNetCore.Http;

namespace Maido.PLGUI.Helpers;

/// <summary>
/// CAPA DE PRESENTACIÓN - HELPER DE SESIÓN: SesionHelper
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es ISession en ASP.NET Core?
///    Es el mecanismo que permite mantener el estado de un usuario entre múltiples peticiones HTTP independientes (stateless).
///    Almacena variables clave en la memoria RAM del servidor asociadas a la cookie `.Maido.Session`.
/// 
/// 2. Control de Acceso Basado en Roles (RBAC):
///    Al iniciar sesión, guardamos el `IdRol` (1 = Administrador, 2 = Cliente).
///    Este Helper proporciona métodos estáticos como `EsAdministrador()` y `EsCliente()` para proteger los Controladores.
/// </summary>
public static class SesionHelper
{
    // Claves privadas para evitar errores tipográficos (Magic Strings) al acceder a la sesión
    private const string KeyIdUsuario  = "Maido_IdUsuario";
    private const string KeyNombre     = "Maido_Nombre";
    private const string KeyEmail      = "Maido_Email";
    private const string KeyIdRol      = "Maido_IdRol";
    private const string KeyNombreRol  = "Maido_NombreRol";

    /// <summary>
    /// Guarda las credenciales esenciales del usuario autenticado en la sesión HTTP.
    /// </summary>
    public static void IniciarSesion(ISession session, int idUsuario, string nombre, string email, int idRol, string nombreRol)
    {
        session.SetInt32(KeyIdUsuario, idUsuario);
        session.SetString(KeyNombre,    nombre);
        session.SetString(KeyEmail,     email);
        session.SetInt32(KeyIdRol,      idRol);
        session.SetString(KeyNombreRol, nombreRol);
    }

    /// <summary>
    /// Limpia todas las variables almacenadas en la sesión (Cierre de sesión / Logout).
    /// </summary>
    public static void CerrarSesion(ISession session) => session.Clear();

    /// <summary>
    /// Verifica si existe un usuario autenticado en la sesión activa.
    /// </summary>
    public static bool EstaAutenticado(ISession session)
        => session.GetInt32(KeyIdUsuario).HasValue;

    /// <summary>
    /// Obtiene el IdUsuario logueado o null si es un visitante anónimo.
    /// </summary>
    public static int? ObtenerIdUsuario(ISession session)
        => session.GetInt32(KeyIdUsuario);

    /// <summary>
    /// Obtiene el nombre formateado del usuario logueado.
    /// </summary>
    public static string ObtenerNombre(ISession session)
        => session.GetString(KeyNombre) ?? string.Empty;

    /// <summary>
    /// Obtiene el correo electrónico del usuario activo.
    /// </summary>
    public static string ObtenerEmail(ISession session)
        => session.GetString(KeyEmail) ?? string.Empty;

    /// <summary>
    /// Obtiene el Id de Rol asignado (1 = Admin, 2 = Cliente).
    /// </summary>
    public static int ObtenerIdRol(ISession session)
        => session.GetInt32(KeyIdRol) ?? 0;

    /// <summary>
    /// Verifica si el usuario autenticado posee privilegios de Administrador (IdRol == 1).
    /// </summary>
    public static bool EsAdministrador(ISession session)
        => ObtenerIdRol(session) == 1;

    /// <summary>
    /// Verifica si el usuario autenticado posee rol de Cliente (IdRol == 2).
    /// </summary>
    public static bool EsCliente(ISession session)
        => ObtenerIdRol(session) == 2;
}

