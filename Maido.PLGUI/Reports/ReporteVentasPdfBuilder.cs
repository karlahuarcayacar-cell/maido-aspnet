using Maido.Application.BL.BC.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Maido.PLGUI.Reports;

/// <summary>
/// CAPA DE PRESENTACIÓN - GENERADOR DE REPORTES PDF (QuestPDF): ReporteVentasPdfBuilder
/// 
/// CONCEPTOS CLAVE PARA EL ESTUDIANTE:
/// 1. ¿Qué es QuestPDF?
///    Es una librería moderna e imperativa de maquetación de documentos PDF para .NET.
///    Permite construir PDFs con componentes fluidos (Header, Content, Footer, Tables, Cards, Charts).
/// 
/// 2. Generación Binaria Dinámica (`byte[]`):
///    El método `Generar(...)` procesa la data agregada de ventas y platillos, construye el diseño vectorial 
///    y retorna una matriz binaria `byte[]` que el Controller entrega al navegador mediante `File(pdfBytes, "application/pdf", filename)`.
/// </summary>
public static class ReporteVentasPdfBuilder
{
    private const string Rojo = "#D9381E";
    private const string Dorado = "#E0A96D";
    private const string Negro = "#0F0F11";
    private const string GrisTexto = "#555555";
    private const string GrisFondo = "#F4F4F6";
    private const string Verde = "#2E7D32";

    /// <summary>
    /// Construye el documento PDF de reporte ejecutivo y devuelve su buffer binario.
    /// </summary>
    public static byte[] Generar(FiltroReporteDto filtro, IEnumerable<VentaPorFechaDto> ventas,
        IEnumerable<PlatilloMasVendidoDto> platillos, string nombreAdmin)

    {
        var listaVentas = ventas.ToList();
        var listaPlatillos = platillos.ToList();

        var ventasTotales = listaVentas.Sum(v => v.MontoTotal);
        var totalPedidos = listaVentas.Sum(v => v.TotalPedidos);
        var ticketPromedio = totalPedidos > 0 ? ventasTotales / totalPedidos : 0m;
        var platilloTop = listaPlatillos.OrderByDescending(p => p.TotalIngresos).FirstOrDefault();
        var diasConActividad = listaVentas.Count(v => v.TotalPedidos > 0);

        var maxMonto = listaVentas.Count > 0 ? listaVentas.Max(v => v.MontoTotal) : 0m;
        var maxIngreso = listaPlatillos.Count > 0 ? listaPlatillos.Max(p => p.TotalIngresos) : 0m;

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(Negro));

                page.Header().Element(e => ComponerHeader(e, nombreAdmin));

                page.Content().PaddingTop(14).Column(col =>
                {
                    col.Spacing(14);

                    col.Item().Element(e => ComponerFiltro(e, filtro));

                    col.Item().Row(row =>
                    {
                        row.Spacing(10);
                        row.RelativeItem().Element(e => TarjetaStat(e, "VENTAS TOTALES",
                            $"S/ {ventasTotales:F2}", $"{diasConActividad} días con actividad", Rojo));
                        row.RelativeItem().Element(e => TarjetaStat(e, "TOTAL PEDIDOS",
                            totalPedidos.ToString(), "Atendidos en periodo", Dorado));
                        row.RelativeItem().Element(e => TarjetaStat(e, "TICKET PROMEDIO",
                            $"S/ {ticketPromedio:F2}", "Por pedido realizado", Rojo));
                        row.RelativeItem().Element(e => TarjetaStat(e, "PLATILLO TOP (INGRESOS)",
                            platilloTop?.NombrePlatillo ?? "—",
                            platilloTop is null ? "Sin datos" : $"S/ {platilloTop.TotalIngresos:F2} ({platilloTop.TotalUnidades} unids)",
                            Dorado));
                    });

                    col.Item().Text("Análisis Gráfico de Desempeño").FontSize(12).Bold();

                    col.Item().Row(row =>
                    {
                        row.Spacing(10);
                        row.RelativeItem().Element(e => GraficoVentas(e, listaVentas, maxMonto));
                        row.RelativeItem().Element(e => GraficoPlatillos(e, listaPlatillos, maxIngreso));
                    });

                    col.Item().Text("Detalle de Resultados").FontSize(12).Bold();

                    col.Item().Row(row =>
                    {
                        row.Spacing(10);
                        row.RelativeItem().Element(e => TablaVentas(e, listaVentas));
                        row.RelativeItem().Element(e => TablaPlatillos(e, listaPlatillos));
                    });
                });

                page.Footer().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Text("Maido Restaurant Management System • Confidential")
                        .FontSize(7).FontColor(GrisTexto);
                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Página ").FontSize(7).FontColor(GrisTexto);
                        x.CurrentPageNumber().FontSize(7).FontColor(GrisTexto);
                        x.Span(" de ").FontSize(7).FontColor(GrisTexto);
                        x.TotalPages().FontSize(7).FontColor(GrisTexto);
                    });
                });
            });
        });

        return documento.GeneratePdf();
    }

    private static void ComponerHeader(IContainer container, string nombreAdmin)
    {
        container.Background(Negro).Padding(16).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("鮨 MAIDO").FontSize(20).Bold().FontColor(Colors.White);
                col.Item().PaddingTop(2).Text("SISTEMA DE GESTIÓN DE PEDIDOS WEB")
                    .FontSize(7).FontColor(Colors.Grey.Lighten2);
            });

            row.ConstantItem(170).Column(col =>
            {
                col.Item().AlignRight().Element(badge =>
                    badge.Background(Rojo).PaddingVertical(3).PaddingHorizontal(8)
                        .Text("REPORTE EJECUTIVO").FontSize(7).Bold().FontColor(Colors.White));
                col.Item().PaddingTop(5).AlignRight()
                    .Text($"Fecha Emisión: {DateTime.Now:dd/MM/yyyy}").FontSize(7).FontColor(Colors.Grey.Lighten2);
                col.Item().AlignRight()
                    .Text($"Generado por: {nombreAdmin}").FontSize(7).FontColor(Colors.Grey.Lighten2);
            });
        });
    }

    private static void ComponerFiltro(IContainer container, FiltroReporteDto filtro)
    {
        container.Column(col =>
        {
            col.Item().Background(GrisFondo).Padding(10)
                .Text("PARÁMETROS DEL FILTRO APLICADO (FiltroReporteDto)")
                .FontSize(7).Bold().FontColor(GrisTexto);

            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span("Fecha Inicio: ").FontSize(8).FontColor(GrisTexto);
                    t.Span(filtro.FechaInicio.ToString("dd/MM/yyyy")).FontSize(8).Bold();
                });
                row.RelativeItem().Text(t =>
                {
                    t.Span("Fecha Fin: ").FontSize(8).FontColor(GrisTexto);
                    t.Span(filtro.FechaFin.ToString("dd/MM/yyyy")).FontSize(8).Bold();
                });
                row.RelativeItem().Text(t =>
                {
                    t.Span("Top N Platillos: ").FontSize(8).FontColor(GrisTexto);
                    t.Span($"{filtro.Top} Registros").FontSize(8).Bold();
                });
            });
        });
    }

    private static void TarjetaStat(IContainer container, string label, string valor, string sub, string colorBarra)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Row(row =>
        {
            row.ConstantItem(3).Background(colorBarra);
            row.RelativeItem().PaddingLeft(6).Column(col =>
            {
                col.Item().Text(label).FontSize(6).Bold().FontColor(GrisTexto);
                col.Item().PaddingTop(2).Text(valor).FontSize(14).Bold();
                col.Item().PaddingTop(2).Text(sub).FontSize(6).FontColor(Verde);
            });
        });
    }

    private static void GraficoVentas(IContainer container, List<VentaPorFechaDto> ventas, decimal maxMonto)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text("Evolución de Ventas por Fecha").FontSize(8).Bold();

            if (ventas.Count == 0)
            {
                col.Item().PaddingTop(30).AlignCenter().Text("Sin datos").FontColor(Colors.Grey.Medium);
                return;
            }

            col.Item().PaddingTop(10).Height(110).Row(row =>
            {
                foreach (var v in ventas)
                {
                    var alturaPct = maxMonto > 0 ? (float)(v.MontoTotal / maxMonto) : 0f;
                    var alturaPx = Math.Max(4, 90 * alturaPct);

                    row.RelativeItem().AlignBottom().Column(barCol =>
                    {
                        barCol.Item().AlignCenter().Text($"S/{v.MontoTotal:F0}").FontSize(6);
                        barCol.Item().AlignCenter().Height(alturaPx).Width(18).Background(Rojo);
                        barCol.Item().AlignCenter().PaddingTop(2).Text(v.Fecha.ToString("dd/MM")).FontSize(6);
                    });
                }
            });
        });
    }

    private static void GraficoPlatillos(IContainer container, List<PlatilloMasVendidoDto> platillos, decimal maxIngreso)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text("Ingresos por Platillo Más Vendido").FontSize(8).Bold();

            if (platillos.Count == 0)
            {
                col.Item().PaddingTop(30).AlignCenter().Text("Sin datos").FontColor(Colors.Grey.Medium);
                return;
            }

            col.Item().PaddingTop(10).Column(inner =>
            {
                foreach (var p in platillos)
                {
                    var anchoPct = maxIngreso > 0 ? (float)(p.TotalIngresos / maxIngreso) : 0.03f;
                    anchoPct = Math.Clamp(anchoPct, 0.03f, 0.85f);

                    inner.Item().PaddingBottom(8).Column(fila =>
                    {
                        fila.Item().Text(p.NombrePlatillo).FontSize(7);
                        fila.Item().PaddingTop(2).Row(barRow =>
                        {
                            barRow.RelativeItem(anchoPct).Height(10).Background(Negro);
                            barRow.RelativeItem(1f - anchoPct).PaddingLeft(4).AlignMiddle()
                                .Text($"S/ {p.TotalIngresos:F2} ({p.TotalUnidades} unids)").FontSize(6);
                        });
                    });
                }
            });
        });
    }

    private static void TablaVentas(IContainer container, List<VentaPorFechaDto> ventas)
    {
        container.Column(col =>
        {
            col.Item().Text("Ventas por Fecha (VentaPorFechaDto)").FontSize(8).Bold();

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Negro).Padding(5).Text("FECHA").FontColor(Colors.White).FontSize(6).Bold();
                    header.Cell().Background(Negro).Padding(5).AlignCenter().Text("PEDIDOS").FontColor(Colors.White).FontSize(6).Bold();
                    header.Cell().Background(Negro).Padding(5).AlignRight().Text("MONTO TOTAL").FontColor(Colors.White).FontSize(6).Bold();
                });

                if (ventas.Count == 0)
                {
                    table.Cell().ColumnSpan(3).Padding(10).AlignCenter()
                        .Text("Sin datos en el periodo.").FontColor(Colors.Grey.Medium).FontSize(7);
                }
                else
                {
                    foreach (var v in ventas)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5)
                            .Text(v.Fecha.ToString("dd/MM/yyyy")).FontSize(7);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5)
                            .AlignCenter().Text(v.TotalPedidos.ToString()).FontSize(7);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5)
                            .AlignRight().Text($"S/ {v.MontoTotal:F2}").FontSize(7);
                    }

                    table.Cell().Background(GrisFondo).Padding(5).Text("TOTALES").Bold().FontSize(7);
                    table.Cell().Background(GrisFondo).Padding(5).AlignCenter()
                        .Text(ventas.Sum(v => v.TotalPedidos).ToString()).Bold().FontSize(7);
                    table.Cell().Background(GrisFondo).Padding(5).AlignRight()
                        .Text($"S/ {ventas.Sum(v => v.MontoTotal):F2}").Bold().FontColor(Rojo).FontSize(7);
                }
            });
        });
    }

    private static void TablaPlatillos(IContainer container, List<PlatilloMasVendidoDto> platillos)
    {
        container.Column(col =>
        {
            col.Item().Text("Platillos Más Vendidos (PlatilloMasVendidoDto)").FontSize(8).Bold();

            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Negro).Padding(5).Text("PLATILLO").FontColor(Colors.White).FontSize(6).Bold();
                    header.Cell().Background(Negro).Padding(5).AlignCenter().Text("UNIDADES").FontColor(Colors.White).FontSize(6).Bold();
                    header.Cell().Background(Negro).Padding(5).AlignRight().Text("INGRESOS").FontColor(Colors.White).FontSize(6).Bold();
                });

                if (platillos.Count == 0)
                {
                    table.Cell().ColumnSpan(3).Padding(10).AlignCenter()
                        .Text("Sin datos en el periodo.").FontColor(Colors.Grey.Medium).FontSize(7);
                }
                else
                {
                    var rank = 1;
                    foreach (var p in platillos)
                    {
                        var colorMedalla = rank switch
                        {
                            1 => Dorado,
                            2 => "#B0B0B0",
                            3 => "#C88D50",
                            _ => "#D9D9D9"
                        };

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Row(r =>
                        {
                            r.ConstantItem(14).Height(14).Background(colorMedalla).AlignMiddle().AlignCenter()
                                .Text(rank.ToString()).FontColor(Colors.White).FontSize(6).Bold();
                            r.RelativeItem().PaddingLeft(5).AlignMiddle().Text(p.NombrePlatillo).FontSize(7);
                        });
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5)
                            .AlignCenter().Text(p.TotalUnidades.ToString()).Bold().FontSize(7);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5)
                            .AlignRight().Text($"S/ {p.TotalIngresos:F2}").Bold().FontSize(7);

                        rank++;
                    }

                    var top3 = platillos.Take(3).ToList();
                    table.Cell().Background(GrisFondo).Padding(5).Text("SUBTOTAL TOP 3").Bold().FontSize(7);
                    table.Cell().Background(GrisFondo).Padding(5).AlignCenter()
                        .Text(top3.Sum(p => p.TotalUnidades).ToString()).Bold().FontSize(7);
                    table.Cell().Background(GrisFondo).Padding(5).AlignRight()
                        .Text($"S/ {top3.Sum(p => p.TotalIngresos):F2}").Bold().FontColor(Dorado).FontSize(7);
                }
            });
        });
    }
}