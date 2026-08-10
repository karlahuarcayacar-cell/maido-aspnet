using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// Controlador del área de clientes autenticados.
/// </summary>
public class ClienteController : Controller
{
    private readonly IPedidoService _pedidoService;

    public ClienteController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // ─────────────────────────────────────────────────────
    // GET: Mis Pedidos
    // ─────────────────────────────────────────────────────
    public async Task<IActionResult> MisPedidos()
    {
        if (!SesionHelper.EstaAutenticado(HttpContext.Session))
            return RedirectToAction("Login", "Account");

        var idUsuario = SesionHelper.ObtenerIdUsuario(HttpContext.Session)!.Value;
        var pedidos   = await _pedidoService.ListarPorUsuarioAsync(idUsuario);

        ViewBag.NombreUsuario = SesionHelper.ObtenerNombre(HttpContext.Session);
        return View(pedidos);
    }

    // ─────────────────────────────────────────────────────
    // GET: Detalle de pedido
    // ─────────────────────────────────────────────────────
    public IActionResult DetallePedido(int id)
    {
        return RedirectToAction("MisPedidos");
    }
}
