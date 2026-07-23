using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Maido.PLGUI.Helpers;

/// <summary>
/// Helper para gestionar la sesión del usuario autenticado.
/// </summary>
public static class SesionHelper
{
    private const string KeyIdUsuario  = "Maido_IdUsuario";
    private const string KeyNombre     = "Maido_Nombre";
    private const string KeyEmail      = "Maido_Email";
    private const string KeyIdRol      = "Maido_IdRol";
    private const string KeyNombreRol  = "Maido_NombreRol";

    public static void IniciarSesion(ISession session, int idUsuario, string nombre, string email, int idRol, string nombreRol)
    {
        session.SetInt32(KeyIdUsuario, idUsuario);
        session.SetString(KeyNombre,    nombre);
        session.SetString(KeyEmail,     email);
        session.SetInt32(KeyIdRol,      idRol);
        session.SetString(KeyNombreRol, nombreRol);
    }

    public static void CerrarSesion(ISession session) => session.Clear();

    public static bool EstaAutenticado(ISession session)
        => session.GetInt32(KeyIdUsuario).HasValue;

    public static int? ObtenerIdUsuario(ISession session)
        => session.GetInt32(KeyIdUsuario);

    public static string ObtenerNombre(ISession session)
        => session.GetString(KeyNombre) ?? string.Empty;

    public static string ObtenerEmail(ISession session)
        => session.GetString(KeyEmail) ?? string.Empty;

    public static int ObtenerIdRol(ISession session)
        => session.GetInt32(KeyIdRol) ?? 0;

    public static bool EsAdministrador(ISession session)
        => ObtenerIdRol(session) == 1;

    public static bool EsCliente(ISession session)
        => ObtenerIdRol(session) == 2;
}
