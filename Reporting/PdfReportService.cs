using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EncryptionHomework.Reporting;

public class PdfReportService
{
    private readonly string reportsDirectory;

    public PdfReportService()
    {
        string launchPath = Path.GetFullPath(Environment.GetCommandLineArgs()[0]);
        string applicationDirectory = Path.GetDirectoryName(launchPath)
            ?? AppContext.BaseDirectory;

        reportsDirectory = Path.Combine(applicationDirectory, "Reports");
    }

    public string SaveReport(string algorithmName, IEnumerable<string> lines)
    {
        Directory.CreateDirectory(reportsDirectory);
        string fileName = $"{algorithmName}_Комаров_Комаров.pdf";
        string fullPath = Path.Combine(reportsDirectory, fileName);
        List<string> reportLines = lines.ToList();

        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.3f, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontFamily("Lato").FontSize(9.5f));

                page.Header()
                    .PaddingBottom(6)
                    .Text($"{algorithmName}: Комаров / Комаров")
                    .SemiBold()
                    .FontSize(15)
                    .FontColor(Colors.Blue.Darken2);

                page.Content().Column(column =>
                {
                    column.Spacing(1.5f);

                    foreach (string line in reportLines)
                    {
                        if (line.Length > 0 && line.All(symbol => symbol == '='))
                        {
                            column.Item().PaddingVertical(3).LineHorizontal(1)
                                .LineColor(Colors.Grey.Lighten1);
                        }
                        else if (line.Length == 0)
                        {
                            column.Item().Height(4);
                        }
                        else if (line == "Результат: УСПЕШНО" || line == "Результат: ОШИБКА")
                        {
                            column.Item().Text(line).SemiBold()
                                .FontColor(line == "Результат: УСПЕШНО"
                                    ? Colors.Green.Darken2
                                    : Colors.Red.Darken2);
                        }
                        else
                        {
                            column.Item().Text(line);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Страница ");
                    text.CurrentPageNumber();
                    text.Span(" из ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf(fullPath);

        return fullPath;
    }
}
