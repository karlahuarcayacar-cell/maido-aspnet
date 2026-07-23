namespace Maido.Domain.BL.BE.Entities;
public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; }
}
