using Maido.Domain.BL.BE.Entities;

namespace Maido.Domain.BL.BE.Interfaces;

/// <summary>
/// CAPA DE DOMINIO - INTERFAZ DE REPOSITORIO: IUsuarioRepository
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es el Patrón Repository (Repositorio)?
///    Es un patrón de diseño que abstrae la persistencia de datos. Define un "contrato" de qué operaciones 
///    de acceso a datos están disponibles, SIN especificar cómo se implementan (ADO.NET, Dapper, EF Core, SQL, MySQL).
/// 
/// 2. Inversión de Dependencias (D de SOLID):
///    El Dominio define esta interfaz. La capa de Infraestructura (Infrastructure) se encarga de implementarla.
///    De este modo, el sistema se vuelve descaloplado y fácil de probar con pruebas unitarias (Unit Tests / Mocks).
/// 
/// 3. Programación Asíncrona (async / Task):
///    Todos los métodos devuelven un `Task` o `Task<T>`, lo que indica que se ejecutarán sin bloquear el hilo principal 
///    de procesamiento mientras esperan la respuesta de la base de datos SQL Server.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca un usuario por su correo electrónico único. Devuelve null si no existe.
    /// </summary>
    Task<Usuario?> ObtenerUsuarioPorEmailAsync(string email);

    /// <summary>
    /// Registra un nuevo usuario en la base de datos llamando al SP `sp_RegistrarUsuario`.
    /// Devuelve el IdUsuario generado (o -1 si el correo ya existe).
    /// </summary>
    Task<int> RegistrarUsuarioAsync(Usuario usuario);

    /// <summary>
    /// Retorna el listado completo de usuarios registrados en el sistema.
    /// </summary>
    Task<IEnumerable<Usuario>> ListarUsuariosAsync();

    /// <summary>
    /// Cambia el estado de activación (Activo = true/false) de un usuario (Borrado lógico / Suspensión).
    /// </summary>
    Task ActualizarEstadoUsuarioAsync(int idUsuario, bool activo);

    /// <summary>
    /// Actualiza la información del perfil (Nombre, Apellido, Teléfono, Dirección) de un usuario existente.
    /// </summary>
    Task ActualizarPerfilUsuarioAsync(Usuario usuario);
}

