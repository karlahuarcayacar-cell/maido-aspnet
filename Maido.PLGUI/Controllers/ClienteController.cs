using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

public class ClienteController : Controller
{
    private readonly IPedidoService _pedidoService;

    public ClienteController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

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
