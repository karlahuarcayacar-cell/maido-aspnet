using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// CAPA DE PRESENTACIÓN - CONTROLADOR DE ÁREA DE CLIENTES: ClienteController
/// 
/// CONCEPTOS PARA EL ESTUDIANTE:
/// Permite al cliente autenticado consultar el historial de todas sus órdenes registradas.
/// </summary>
public class ClienteController : Controller
{
    private readonly IPedidoService _pedidoService;

    public ClienteController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    /// <summary>
    /// [GET] Muestra el historial de compras del cliente logueado.
    /// Valida que exista una sesión activa antes de consultar la base de datos.
    /// </summary>
    public async Task<IActionResult> MisPedidos()
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account");

        var idUsuario = SesionHelper.ObtenerIdUsuario(HttpContext.Session)!.Value;
        var pedidos   = await _pedidoService.ListarPorUsuarioAsync(idUsuario);

        ViewBag.NombreUsuario = SesionHelper.ObtenerNombre(HttpContext.Session);
        return View(pedidos);
    }
}

