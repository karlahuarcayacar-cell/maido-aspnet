using Maido.Domain.BL.BE.Entities;
namespace Maido.Domain.BL.BE.Interfaces;
public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerUsuarioPorEmailAsync(string email);
    Task<int> RegistrarUsuarioAsync(Usuario usuario);
    Task<IEnumerable<Usuario>> ListarUsuariosAsync();
    Task ActualizarEstadoUsuarioAsync(int idUsuario, bool activo);
    Task ActualizarPerfilUsuarioAsync(Usuario usuario);
}
