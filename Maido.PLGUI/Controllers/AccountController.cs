using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// Controlador de autenticación: Login y Registro.
/// </summary>
public class AccountController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public AccountController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    // ─────────────────────────────────────────────────────
    // GET: Login
    // ─────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToHome();

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // ─────────────────────────────────────────────────────
    // POST: Login
    // ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        var usuario = await _usuarioService.AutenticarAsync(dto);
        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas. Verifique su email y contraseña.");
            ViewBag.ReturnUrl = returnUrl;
            return View(dto);
        }

        SesionHelper.IniciarSesion(
            HttpContext.Session,
            usuario.IdUsuario,
            usuario.NombreCompleto,
            usuario.Email,
            usuario.IdRol,
            usuario.NombreRol);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToHome();
    }

    // ─────────────────────────────────────────────────────
    // GET: Registro
    // ─────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Register()
    {
        if (SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToHome();

        return View();
    }

    // ─────────────────────────────────────────────────────
    // POST: Registro
    // ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrarUsuarioDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (dto.Password != dto.ConfirmarPassword)
        {
            ModelState.AddModelError("ConfirmarPassword", "Las contraseñas no coinciden.");
            return View(dto);
        }

        var (exitoso, mensaje, _) = await _usuarioService.RegistrarAsync(dto);
        if (!exitoso)
        {
            ModelState.AddModelError(string.Empty, mensaje);
            return View(dto);
        }

        TempData["Exito"] = "Cuenta creada correctamente. ¡Inicia sesión para continuar!";
        return RedirectToAction("Login");
    }

    // ─────────────────────────────────────────────────────
    // GET: Perfil
    // ─────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────
    // POST: Perfil
    // ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilDto dto)
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login");

        if (!ModelState.IsValid)
            return View(dto);

        // Seguridad: Asegurar que el IdUsuario coincida con la sesión
        var sessionId = HttpContext.Session.GetInt32("Maido_IdUsuario");
        if (sessionId != dto.IdUsuario)
            return RedirectToAction("Login");

        await _usuarioService.ActualizarPerfilAsync(dto);

        // Actualizar la sesión por si cambió su nombre
        HttpContext.Session.SetString("Maido_Nombre", $"{dto.Nombre} {dto.Apellido}");

        TempData["Exito"] = "Perfil actualizado correctamente.";
        return RedirectToAction("Perfil");
    }

    // ─────────────────────────────────────────────────────
    // POST: Logout
    // ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        SesionHelper.CerrarSesion(HttpContext.Session);
        return RedirectToAction("Index", "Home");
    }

    // ─────────────────────────────────────────────────────
    // Redirección según rol
    // ─────────────────────────────────────────────────────
    private IActionResult RedirectToHome()
    {
        if (SesionHelper.EsAdministrador(HttpContext.Session))
            return RedirectToAction("Dashboard", "Admin");

        return RedirectToAction("Index", "Home");
    }
}
