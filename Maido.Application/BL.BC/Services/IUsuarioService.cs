using Maido.Application.BL.BC.DTOs;

namespace Maido.Application.BL.BC.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> AutenticarAsync(LoginDto dto);
    Task<(bool Exitoso, string Mensaje, int IdUsuario)> RegistrarAsync(RegistrarUsuarioDto dto);
    Task<IEnumerable<UsuarioDto>> ListarAsync();
    Task ActualizarEstadoAsync(int idUsuario, bool activo);
    Task<PerfilDto?> ObtenerPerfilPorEmailAsync(string email);
    Task ActualizarPerfilAsync(PerfilDto dto);
}
