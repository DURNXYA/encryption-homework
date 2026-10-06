using QuestPDF.Infrastructure;
using EncryptionHomework.Application;
using EncryptionHomework.Reporting;

namespace EncryptionHomework;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        if (args.Contains("--demo"))
        {
            GenerateDemoReports();
            return;
        }

        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new MainForm());
    }

    private static void GenerateDemoReports()
    {
        var pdf = new PdfReportService();
        pdf.SaveReport("Цезарь", EncryptionScenarios.Caesar("1709", "482", 3));
        pdf.SaveReport(
            "Виженер",
            EncryptionScenarios.Vigenere("ПРИВЕТ", "СТУДЕНТ", "МОЙ КЛЮЧ"));
        pdf.SaveReport("XOR", EncryptionScenarios.Xor(25, 42, 13));
        pdf.SaveReport(
            "RSA",
            EncryptionScenarios.Rsa("КОДЕР", "ШИФР", 5, 11, 3, 13, 17, 5));
        pdf.SaveReport(
            "Плейфер",
            EncryptionScenarios.Playfair("HELLO", "WORLD", "MONARCHY", "STUDENT"));
    }
}
