using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// CAPA DE PRESENTACIÓN - CONTROLADOR MVC: AccountController
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es un Controller en ASP.NET Core MVC?
///    Es la clase encargada de recibir las peticiones HTTP del usuario, invocar la lógica de los Servicios 
///    y seleccionar la Vista (.cshtml) o redirección correspondiente.
/// 
/// 2. Atributos de Métodos de Acción (Action Attributes):
///    - `[HttpGet]`: Responde a solicitudes de lectura (cargar una página o formulario en el navegador).
///    - `[HttpPost]`: Responde al envío de datos mediante formularios o peticiones AJAX.
///    - `[ValidateAntiForgeryToken]`: MÉTODO DE SEGURIDAD CRÍTICO. Protege contra ataques CSRF (Cross-Site Request Forgery) 
///      validando una clave secreta oculta generada por `@Html.AntiForgeryToken()` en la Vista.
/// 
/// 3. Formas de Enviar Datos a las Vistas:
///    - **Modelos Fuertemente Tipados**: Pasar un objeto como parámetro a `View(model)` (ej: `View(dto)`). Es la mejor práctica.
///    - **ViewBag**: Objeto dinámico (`dynamic`) para enviar valores rápidos y no tipados a la vista.
///    - **TempData**: Almacenamiento temporal que sobrevive a una redirección HTTP (`RedirectToAction`). Ideal para mensajes de éxito/error.
/// </summary>
public class AccountController : Controller
{
    private readonly IUsuarioService _usuarioService;

    /// <summary>
    /// Inyección de la interfaz del servicio de usuario `IUsuarioService`.
    /// </summary>
    public AccountController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// [GET] Muestra la pantalla de Login.
    /// Si el usuario ya inició sesión previamente, lo redirige automáticamente a la Home o al Dashboard Admin.
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToHome();

        TempData.Remove("Exito");

        // Enviar la URL de retorno original mediante ViewBag
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// [POST] Procesa las credenciales de Login ingresadas.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
    {
        // Validar anotaciones de datos en el DTO
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        // Invocar el servicio de autenticación con Hashing SHA-256
        var usuario = await _usuarioService.AutenticarAsync(dto);
        if (usuario is null)
        {
            // Agregar error global al formulario para mostrar al usuario
            ModelState.AddModelError(string.Empty, "Credenciales inválidas. Verifique su email y contraseña.");
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        // Crear las variables de sesión del usuario
        SesionHelper.IniciarSesion(
            HttpContext.Session,
            usuario.IdUsuario,
            usuario.NombreCompleto,
            usuario.Email,
            usuario.IdRol,
            usuario.NombreRol);

        // Prevenir ataques de Redirección Abierta verificando `Url.IsLocalUrl`
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToHome();
    }

    /// <summary>
    /// [GET] Muestra el formulario de registro de nuevos clientes.
    /// </summary>
    [HttpGet]
    public IActionResult Register()
    {
        if (SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToHome();

        return View();
    }

    /// <summary>
    /// [POST] Procesa la creación de la cuenta de un nuevo cliente.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrarUsuarioDto dto)
    {
        if (string.IsNullOrEmpty(dto.ConfirmarPassword))
        {
            dto.ConfirmarPassword = dto.Password;
        }

        if (!ModelState.IsValid)
            return View(dto);

        if (dto.Password != dto.ConfirmarPassword)
        {
            ModelState.AddModelError("ConfirmarPassword", "Las contraseñas no coinciden.");
            return View(dto);
        }

        var (exitoso, mensaje, idUsuario) = await _usuarioService.RegistrarAsync(dto);
        if (!exitoso)
        {
            ModelState.AddModelError(string.Empty, mensaje);
            return View(dto);
        }

        // Auto-login tras registro exitoso
        var usuario = await _usuarioService.AutenticarAsync(new LoginDto { Email = dto.Email, Password = dto.Password });
        if (usuario != null)
        {
            SesionHelper.IniciarSesion(
                HttpContext.Session,
                usuario.IdUsuario,
                usuario.NombreCompleto,
                usuario.Email,
                usuario.IdRol,
                usuario.NombreRol);
            TempData["AuthExito"] = "¡Cuenta creada exitosamente! Bienvenido a Maido.";
            return RedirectToAction("Index", "Home");
        }

        TempData["AuthExito"] = "Cuenta creada correctamente. ¡Inicia sesión para continuar!";
        return RedirectToAction("Login");
    }

    /// <summary>
    /// [GET] Carga la pantalla de edición del perfil del usuario en sesión.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login");

        var email = HttpContext.Session.GetString("Maido_Email");
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        var perfil = await _usuarioService.ObtenerPerfilPorEmailAsync(email);
        if (perfil == null)
            return RedirectToAction("Login");

        return View(perfil);
    }

    /// <summary>
    /// [POST] Guarda las modificaciones realizadas al perfil del usuario.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilDto dto)
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login");

        if (!ModelState.IsValid)
            return View(dto);

        // Control de Seguridad IDOR: Validar que el IdUsuario a modificar pertenezca al usuario en sesión
        var sessionId = HttpContext.Session.GetInt32("Maido_IdUsuario");
        if (sessionId != dto.IdUsuario)
            return RedirectToAction("Login");

        await _usuarioService.ActualizarPerfilAsync(dto);

        // Actualizar el nombre visible en la variable de sesión
        HttpContext.Session.SetString("Maido_Nombre", $"{dto.Nombre} {dto.Apellido}");

        TempData["Exito"] = "Perfil actualizado correctamente.";
        return RedirectToAction("Perfil");
    }

    /// <summary>
    /// [POST] Cierra la sesión activa del usuario actual.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        SesionHelper.CerrarSesion(HttpContext.Session);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Helper privado de redirección según el Rol de la sesión.
    /// </summary>
    private IActionResult RedirectToHome()
    {
        if (SesionHelper.EsAdministrador(HttpContext.Session))
            return RedirectToAction("Dashboard", "Admin");

        return RedirectToAction("Index", "Home");
    }
}

