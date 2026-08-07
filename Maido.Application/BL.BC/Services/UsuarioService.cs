using System.Security.Cryptography;
using System.Text;
using Maido.Application.BL.BC.DTOs;
using Maido.Domain.BL.BE.Entities;
using Maido.Domain.BL.BE.Interfaces;

namespace Maido.Application.BL.BC.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo) => _repo = repo;

    public async Task<UsuarioDto?> AutenticarAsync(LoginDto dto)
    {
        var u = await _repo.ObtenerUsuarioPorEmailAsync(dto.Email);
        if (u is null || !u.Activo) return null;

        if (u.PasswordHash != HashPassword(dto.Password)) return null;

        return MapDto(u);
    }

    public async Task<(bool Exitoso, string Mensaje, int IdUsuario)> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        if (dto.Password != dto.ConfirmarPassword)
            return (false, "Las contraseñas no coinciden.", 0);

        var usuario = new Usuario
        {
            Nombre       = dto.Nombre,
            Apellido     = dto.Apellido,
            Email        = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Telefono     = dto.Telefono,
            IdRol        = 2,
            Activo       = true,
            FechaRegistro = DateTime.Now
        };

        var id = await _repo.RegistrarUsuarioAsync(usuario);
        if (id == -1)
            return (false, "El correo electrónico ya está registrado.", 0);

        return (true, "Cuenta creada correctamente.", id);
    }

    public async Task<IEnumerable<UsuarioDto>> ListarAsync()
    {
        var lista = await _repo.ListarUsuariosAsync();
        return lista.Select(MapDto);
    }

    public async Task ActualizarEstadoAsync(int idUsuario, bool activo)
        => await _repo.ActualizarEstadoUsuarioAsync(idUsuario, activo);

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

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

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
