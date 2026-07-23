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
