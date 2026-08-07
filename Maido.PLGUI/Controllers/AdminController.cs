using Maido.Application.BL.BC.DTOs;
using Maido.Application.BL.BC.Services;
using Maido.PLGUI.Helpers;
using Maido.PLGUI.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Maido.PLGUI.Controllers;

/// <summary>
/// Controlador administrativo. Requiere rol Administrador (IdRol == 1).
/// Maneja: Dashboard, Platillos, Categorías, Pedidos y Reportes.
/// </summary>
public class AdminController : Controller
{
    private readonly IPlatilloService _platilloService;
    private readonly ICategoriaService _categoriaService;
    private readonly IPedidoService _pedidoService;
    private readonly IUsuarioService _usuarioService;
    private readonly IReporteService _reporteService;
    private readonly IWebHostEnvironment _env;

    public AdminController(
        IPlatilloService platilloService,
        ICategoriaService categoriaService,
        IPedidoService pedidoService,
        IUsuarioService usuarioService,
        IReporteService reporteService,
        IWebHostEnvironment env)
    {
        _platilloService = platilloService;
        _categoriaService = categoriaService;
        _pedidoService = pedidoService;
        _usuarioService = usuarioService;
        _reporteService = reporteService;
        _env = env;
    }

    // ─────────────────────────────────────────────────────
    // Guard: solo Admin
    // ─────────────────────────────────────────────────────
    private bool EsAdmin()
        => SesionHelper.EstaAutenticado(HttpContext.Session)
           && SesionHelper.EsAdministrador(HttpContext.Session);

    private IActionResult AccesoDenegado()
        => RedirectToAction("Login", "Account");

    // ═══════════════════════════════════════════════════
    // DASHBOARD
    // ═══════════════════════════════════════════════════
    public async Task<IActionResult> Dashboard()
    {
        if (!EsAdmin()) return AccesoDenegado();

        var (pedidos, _) = await _pedidoService.ListarPaginadoAsync(1, 5, null, null, null);
        ViewBag.UltimosPedidos = pedidos;
        ViewBag.NombreAdmin = SesionHelper.ObtenerNombre(HttpContext.Session);

        return View();
    }

    // ═══════════════════════════════════════════════════
    // PLATILLOS
    // ═══════════════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Platillos(int pagina = 1, int? idCategoria = null, string? busqueda = null)
    {
        if (!EsAdmin()) return AccesoDenegado();

        var resultado = await _platilloService.ListarPaginadoAsync(pagina, 10, idCategoria, busqueda);
        var categorias = await _categoriaService.ListarTodasAsync();

        ViewBag.Categorias = categorias;
        ViewBag.IdCategoriaActual = idCategoria;
        ViewBag.Busqueda = busqueda;

        return View(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> CrearPlatillo()
    {
        if (!EsAdmin()) return AccesoDenegado();
        ViewBag.Categorias = await _categoriaService.ListarPublicasAsync();
        return View(new CrearPlatilloDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPlatillo(CrearPlatilloDto dto, IFormFile? imagenFile)
    {
        if (!EsAdmin()) return AccesoDenegado();

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            ModelState.AddModelError("Descripcion", "La descripción del platillo es obligatoria.");

        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await _categoriaService.ListarPublicasAsync();
            return View(dto);
        }

        dto.ImagenUrl = await GuardarImagenAsync(imagenFile);

        await _platilloService.CrearAsync(dto);
        TempData["Exito"] = "Platillo creado correctamente.";
        return RedirectToAction("Platillos");
    }

    [HttpGet]
    public async Task<IActionResult> EditarPlatillo(int id)
    {
        if (!EsAdmin()) return AccesoDenegado();

        var platillo = await _platilloService.ObtenerPorIdAsync(id);
        if (platillo is null) return NotFound();

        ViewBag.Categorias = await _categoriaService.ListarPublicasAsync();
        var dto = new ActualizarPlatilloDto
        {
            IdPlatillo = platillo.IdPlatillo,
            Nombre = platillo.Nombre,
            Descripcion = platillo.Descripcion,
            Precio = platillo.Precio,
            ImagenUrl = platillo.ImagenUrl,
            IdCategoria = platillo.IdCategoria,
            Disponible = platillo.Disponible,
            Destacado = platillo.Destacado
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPlatillo(ActualizarPlatilloDto dto, IFormFile? imagenFile)
    {
        if (!EsAdmin()) return AccesoDenegado();

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            ModelState.AddModelError("Descripcion", "La descripción del platillo es obligatoria.");

        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await _categoriaService.ListarPublicasAsync();
            return View(dto);
        }

        if (imagenFile is not null)
            dto.ImagenUrl = await GuardarImagenAsync(imagenFile);

        await _platilloService.ActualizarAsync(dto);
        TempData["Exito"] = "Platillo actualizado correctamente.";
        return RedirectToAction("Platillos");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarPlatillo(int id)
    {
        if (!EsAdmin()) return AccesoDenegado();
        await _platilloService.EliminarAsync(id);
        TempData["Exito"] = "Platillo eliminado correctamente.";
        return RedirectToAction("Platillos");
    }

    // ═══════════════════════════════════════════════════
    // CATEGORÍAS
    // ═══════════════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Categorias()
    {
        if (!EsAdmin()) return AccesoDenegado();
        var categorias = await _categoriaService.ListarTodasAsync();
        return View(categorias);
    }

    [HttpGet]
    public IActionResult CrearCategoria()
    {
        if (!EsAdmin()) return AccesoDenegado();
        return View(new CrearCategoriaDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCategoria(CrearCategoriaDto dto)
    {
        if (!EsAdmin()) return AccesoDenegado();
        if (!ModelState.IsValid) return View(dto);
        await _categoriaService.CrearAsync(dto);
        TempData["Exito"] = "Categoría creada correctamente.";
        return RedirectToAction("Categorias");
    }

    [HttpGet]
    public async Task<IActionResult> EditarCategoria(int id)
    {
        if (!EsAdmin()) return AccesoDenegado();
        var cat = await _categoriaService.ObtenerPorIdAsync(id);
        if (cat is null) return NotFound();

        var dto = new ActualizarCategoriaDto
        {
            IdCategoria = cat.IdCategoria,
            Nombre = cat.Nombre,
            Descripcion = cat.Descripcion,
            Icono = cat.Icono,
            Orden = cat.Orden,
            Activo = cat.Activo
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCategoria(ActualizarCategoriaDto dto)
    {
        if (!EsAdmin()) return AccesoDenegado();
        if (!ModelState.IsValid) return View(dto);
        await _categoriaService.ActualizarAsync(dto);
        TempData["Exito"] = "Categoría actualizada correctamente.";
        return RedirectToAction("Categorias");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarCategoria(int id)
    {
        if (!EsAdmin()) return AccesoDenegado();
        await _categoriaService.EliminarAsync(id);
        TempData["Exito"] = "Categoría eliminada correctamente.";
        return RedirectToAction("Categorias");
    }

    // ═══════════════════════════════════════════════════
    // PEDIDOS
    // ═══════════════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Pedidos(int pagina = 1, string? estado = null,
        DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        if (!EsAdmin()) return AccesoDenegado();

        var (items, total) = await _pedidoService.ListarPaginadoAsync(pagina, 10, estado, fechaInicio, fechaFin);

        var dto = new PedidosPaginadoDto
        {
            Items = items,
            TotalRegistros = total,
            PaginaActual = pagina,
            RegistrosPorPagina = 10
        };

        ViewBag.EstadoActual = estado;
        ViewBag.FechaInicio = fechaInicio;
        ViewBag.FechaFin = fechaFin;

        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> DetallePedido(int id)
    {
        if (!EsAdmin()) return AccesoDenegado();
        var pedido = await _pedidoService.ObtenerDetalleAsync(id);
        if (pedido is null) return NotFound();
        return View(pedido);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarEstadoPedido(int idPedido, string estado)
    {
        if (!EsAdmin()) return AccesoDenegado();
        await _pedidoService.ActualizarEstadoAsync(idPedido, estado);
        TempData["Exito"] = "Estado del pedido actualizado.";
        return RedirectToAction("DetallePedido", new { id = idPedido });
    }

    // ═══════════════════════════════════════════════════
    // USUARIOS
    // ═══════════════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Usuarios()
    {
        if (!EsAdmin()) return AccesoDenegado();
        var usuarios = await _usuarioService.ListarAsync();
        return View(usuarios);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUsuario(int idUsuario, bool activo)
    {
        if (!EsAdmin()) return AccesoDenegado();
        await _usuarioService.ActualizarEstadoAsync(idUsuario, activo);
        TempData["Exito"] = $"Usuario {(activo ? "activado" : "desactivado")} correctamente.";
        return RedirectToAction("Usuarios");
    }

    // ═══════════════════════════════════════════════════
    // REPORTES
    // ═══════════════════════════════════════════════════
    [HttpGet]
    public async Task<IActionResult> Reportes(FiltroReporteDto? filtro)
    {
        if (!EsAdmin()) return AccesoDenegado();

        filtro ??= new FiltroReporteDto();
        var ventas = await _reporteService.ReporteVentasAsync(filtro.FechaInicio, filtro.FechaFin);
        var platillos = await _reporteService.PlatillosMasVendidosAsync(filtro.FechaInicio, filtro.FechaFin, filtro.Top);

        ViewBag.Ventas = ventas;
        ViewBag.Platillos = platillos;
        ViewBag.Filtro = filtro;

        return View(filtro);
    }

    // ─────────────────────────────────────────────────────
    // GET: Descargar reporte ejecutivo en PDF
    // ─────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> ReportePdf(FiltroReporteDto? filtro)
    {
        if (!EsAdmin()) return AccesoDenegado();

        filtro ??= new FiltroReporteDto();
        var ventas = await _reporteService.ReporteVentasAsync(filtro.FechaInicio, filtro.FechaFin);
        var platillos = await _reporteService.PlatillosMasVendidosAsync(filtro.FechaInicio, filtro.FechaFin, filtro.Top);
        var nombreAdmin = SesionHelper.ObtenerNombre(HttpContext.Session) ?? "Admin Maido";

        var pdfBytes = ReporteVentasPdfBuilder.Generar(filtro, ventas, platillos, nombreAdmin);
        var nombreArchivo = $"reporte_maido_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

        return File(pdfBytes, "application/pdf", nombreArchivo);
    }

    // ═══════════════════════════════════════════════════
    // Helper: Guardar imagen en wwwroot/uploads/platillos/
    // ═══════════════════════════════════════════════════
    private async Task<string?> GuardarImagenAsync(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0)
            return null;

        var extensiones = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (!extensiones.Contains(ext))
            return null;

        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "platillos");
        Directory.CreateDirectory(uploadsPath);

        var nombreArchivo = $"{Guid.NewGuid()}{ext}";
        var rutaCompleta = Path.Combine(uploadsPath, nombreArchivo);

        using var stream = new FileStream(rutaCompleta, FileMode.Create);
        await archivo.CopyToAsync(stream);

        return $"/uploads/platillos/{nombreArchivo}";
    }
}
