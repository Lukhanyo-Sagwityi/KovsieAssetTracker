using KovsieAssetTracker.Data;
using KovsieAssetTracker.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;
using QuestPDF;

[Authorize]
public class ReportsController : Controller
{
    private readonly IAssetRepository _assetRepo;
    public ReportsController(IAssetRepository repo) { _assetRepo = repo; }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> AssetReport(string? location)
    {
        try
        {
            // Defensive: ensure license is set (harmless if already set in Program.cs)
            QuestPDF.Settings.License = LicenseType.Community;

            var assets = (await _assetRepo.SearchAsync(null, location, null)).ToList();

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Kovsie Asset Tracker").SemiBold().FontSize(16);
                                col.Item().Text($"Report generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm}").FontSize(9);
                            });
                            row.ConstantItem(100).AlignRight().Text("UFS / Kovsie").FontSize(10);
                        });

                    page.Content().Column(col =>
                    {
                        col.Item().LineHorizontal(1);
                        col.Item().PaddingVertical(5).Text($"Assets (Location filter: {location ?? "All"})").Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(60);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Tag");
                                header.Cell().Element(CellStyle).Text("Name");
                                header.Cell().Element(CellStyle).Text("Location");
                                header.Cell().Element(CellStyle).Text("Condition");
                                header.Cell().Element(CellStyle).Text("Verified");
                            });

                            foreach (var a in assets)
                            {
                                table.Cell().Element(CellStyle).Text(a.TagId ?? "");
                                table.Cell().Element(CellStyle).Text(a.Name ?? "");
                                table.Cell().Element(CellStyle).Text(a.Location ?? "");
                                table.Cell().Element(CellStyle).Text(a.Condition ?? "");
                                table.Cell().Element(CellStyle).Text(a.IsVerified ? "Yes" : "No");
                            }

                            static IContainer CellStyle(IContainer c) => c.PaddingVertical(3).PaddingHorizontal(5).BorderBottom(1);
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();   // write current page number (descriptor)
                        text.Span(" / ");
                        text.TotalPages();          // write total pages (descriptor)
                    });
                });
            });

            var pdfBytes = doc.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"assets-report-{DateTime.UtcNow:yyyyMMddHHmm}.pdf");
        }
        catch (Exception ex)
        {
            // Return friendly error back to the UI for debugging
            TempData["PdfError"] = "PDF generation failed: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

}
