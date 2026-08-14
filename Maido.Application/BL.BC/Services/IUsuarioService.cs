using Maido.Application.BL.BC.DTOs;

namespace Maido.Application.BL.BC.Services;

/// <summary>
/// CAPA DE APLICACIÓN - INTERFAZ DE SERVICIO: IUsuarioService
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Define la lógica de negocio relativa al ciclo de vida de Usuarios (Autenticación, Registro, Perfil, Listado).
/// Orquesta la llamada a repositorios, el hashing criptográfico de contraseñas y la conversión de Entidades a DTOs.
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Verifica las credenciales de Login (Email y Password) comparando el hash SHA-256.
    /// Devuelve el DTO del usuario si es válido y está activo; de lo contrario null.
    /// </summary>
    Task<UsuarioDto?> AutenticarAsync(LoginDto dto);

    /// <summary>
    /// Valida las reglas de negocio para el registro (coincidencia de clave, email duplicado) 
    /// y guarda el nuevo usuario con contraseña hasheada.
    /// </summary>
    Task<(bool Exitoso, string Mensaje, int IdUsuario)> RegistrarAsync(RegistrarUsuarioDto dto);

    /// <summary>
    /// Obtiene la lista completa de usuarios mapeada a DTOs para el panel de administración.
    /// </summary>
    Task<IEnumerable<UsuarioDto>> ListarAsync();

    /// <summary>
    /// Modifica el estado Habilitado/Deshabilitado de un usuario en el sistema.
    /// </summary>
    Task ActualizarEstadoAsync(int idUsuario, bool activo);

    /// <summary>
    /// Obtiene la información del perfil personal de un cliente logueado a través de su email.
    /// </summary>
    Task<PerfilDto?> ObtenerPerfilPorEmailAsync(string email);

    /// <summary>
    /// Actualiza los datos personales (Teléfono, Dirección, Nombre) desde el perfil.
    /// </summary>
    Task ActualizarPerfilAsync(PerfilDto dto);
}

