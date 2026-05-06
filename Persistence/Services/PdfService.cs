using Application.CarbonReports.Interfaces;
using Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace Persistence.Services;

public class PdfService : IPdfService
{
    private const double DieselFactor = 2.67;
    private const double GasFactor = 0.202;
    private const double ElectricityFactor = 0.420;

    public byte[] GenerateCarbonReportPdf(CarbonReport carbonReport)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        byte[] logoBytes = GenerateInitialLogo(carbonReport.CompanyName);
        byte[] chartBytes = GenerateScopeChart(carbonReport.Co2Scope1, carbonReport.Co2Scope2);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                // --- HEADER: Name links, Logo rechts ---
                page.Header().Row(row =>
                {
                    row.ConstantItem(50).Height(50).Image(logoBytes);
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(carbonReport.CompanyName).FontSize(25).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"CO2-Emissionsbericht | ID: {carbonReport.Id}").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    
                });

                // --- CONTENT ---
                page.Content().PaddingVertical(20).Column(col =>
                {
                    // 1. Tabelle
                    col.Item().PaddingBottom(10).Text("Berechnungsübersicht").FontSize(14).SemiBold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); 
                            columns.RelativeColumn(2); 
                            columns.RelativeColumn(1); 
                            columns.RelativeColumn(2); 
                        });

                        table.Header(header =>
                        {
                            var headerStyle = TextStyle.Default.SemiBold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Kategorie").Style(headerStyle);
                            header.Cell().BorderBottom(1).Padding(5).AlignRight().Text("Menge").Style(headerStyle);
                            header.Cell().BorderBottom(1).Padding(5).AlignRight().Text("Faktor").Style(headerStyle);
                            header.Cell().BorderBottom(1).Padding(5).AlignRight().Text("Emissionen").Style(headerStyle);
                        });

                        AddTableRow(table, "Diesel (Scope 1)", $"{carbonReport.DieselLiters:N0} L", DieselFactor, carbonReport.DieselLiters * DieselFactor / 1000);
                        AddTableRow(table, "Erdgas (Scope 1)", $"{carbonReport.NaturalGasKWh:N0} kWh", GasFactor, carbonReport.NaturalGasKWh * GasFactor / 1000);
                        AddTableRow(table, "Strom (Scope 2)", $"{carbonReport.ElectricityKWh:N0} kWh", ElectricityFactor, carbonReport.ElectricityKWh * ElectricityFactor / 1000);

                        table.Cell().ColumnSpan(3).PaddingTop(10).Text("Summe Scope 1").SemiBold();
                        table.Cell().PaddingTop(10).AlignRight().Text($"{(carbonReport.Co2Scope1 / 1000):N3} t").SemiBold();

                        table.Cell().ColumnSpan(3).Text("Summe Scope 2");
                        table.Cell().AlignRight().Text($"{(carbonReport.Co2Scope2 / 1000):N3} t");

                        table.Cell().ColumnSpan(3).PaddingTop(5).Background(Colors.Blue.Lighten5).Padding(5).Text("Gesamtemissionen").SemiBold().FontColor(Colors.Blue.Medium);
                        table.Cell().PaddingTop(5).Background(Colors.Blue.Lighten5).Padding(5).AlignRight().Text($"{((carbonReport.Co2Scope1 + carbonReport.Co2Scope2) / 1000):N3} tCO2e").SemiBold().FontColor(Colors.Blue.Medium);
                    });

                    // --- DIAGRAMM SEKTION: Legende links, Chart rechts ---
                    col.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().PaddingRight(20).Column(c =>
                        {
                            c.Item().Text("Verteilung nach Scopes").FontSize(13).SemiBold();
                            
                            c.Item().PaddingTop(10).Row(r => {
                                r.ConstantItem(12).Height(12).Background("#2196F3");
                                r.RelativeItem().PaddingLeft(5).Text("Scope 1 (Direkt)").FontSize(10).SemiBold();
                            });
                            c.Item().PaddingLeft(17).Text($"{(carbonReport.Co2Scope1/1000):N2} tCO2e").FontSize(10).FontColor(Colors.Grey.Darken2);

                            c.Item().PaddingTop(10).Row(r => {
                                r.ConstantItem(12).Height(12).Background("#BBDEFB");
                                r.RelativeItem().PaddingLeft(5).Text("Scope 2 (Indirekt)").FontSize(10).SemiBold();
                            });
                            c.Item().PaddingLeft(17).Text($"{(carbonReport.Co2Scope2/1000):N2} tCO2e").FontSize(10).FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(140).Height(140).Image(chartBytes);
                    });
                });

                // --- FOOTER ---
                page.Footer().PaddingTop(10).Column(col =>
                {
                    col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingTop(5).Text("Dieser Bericht wurde maschinell erstellt und ist ohne Unterschrift gültig.").FontSize(8).Italic().FontColor(Colors.Grey.Medium);

                    col.Item().Row(row =>
                    {
                        //row.RelativeItem().Text($"Erstellungsdatum: {DateTime.Now:dd.MM.yyyy}").FontSize(8).FontColor(Colors.Grey.Medium);
                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Seite ").FontSize(8).FontColor(Colors.Grey.Medium);
                            x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                            x.Span(" von ").FontSize(8).FontColor(Colors.Grey.Medium);
                            x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });
                });
            });
        }).GeneratePdf();
    }

    private void AddTableRow(TableDescriptor table, string label, string amount, double factor, double result)
    {
        table.Cell().Padding(5).Text(label);
        table.Cell().Padding(5).AlignRight().Text(amount);
        table.Cell().Padding(5).AlignRight().Text($"{factor:N3}");
        table.Cell().Padding(5).AlignRight().Text($"{result:N3} t");
    }

    private byte[] GenerateInitialLogo(string name)
    {
        string initials = string.IsNullOrWhiteSpace(name) ? "?" : name[..1].ToUpper();
        int hash = name?.GetHashCode() ?? 0;
        var color = SKColor.FromHsl(Math.Abs(hash % 360), 60, 50);

        using var surface = SKSurface.Create(new SKImageInfo(100, 100));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        using var paint = new SKPaint { Color = color, IsAntialias = true };
        canvas.DrawCircle(50, 50, 48, paint);
        using var textPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
        using var font = new SKFont { Size = 50, Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold) };
        canvas.DrawText(initials, 50, 68, SKTextAlign.Center, font, textPaint);
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private byte[] GenerateScopeChart(double s1, double s2)
    {
        using var surface = SKSurface.Create(new SKImageInfo(400, 400));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        double total = s1 + s2;
        if (total <= 0) return Array.Empty<byte>();
        float s1Angle = (float)(s1 / total * 360);
        var rect = new SKRect(20, 20, 380, 380);
        using var p1 = new SKPaint { Color = SKColor.Parse("#2196F3"), IsAntialias = true };
        canvas.DrawArc(rect, -90, s1Angle, true, p1);
        using var p2 = new SKPaint { Color = SKColor.Parse("#BBDEFB"), IsAntialias = true };
        canvas.DrawArc(rect, s1Angle - 90, 360 - s1Angle, true, p2);
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
