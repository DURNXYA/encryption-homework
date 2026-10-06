using EncryptionHomework.Application;
using EncryptionHomework.Reporting;

namespace EncryptionHomework;

public class MainForm : Form
{
    private readonly PdfReportService pdfReportService = new();

    private TextBox caesarFirst = null!;
    private TextBox caesarSecond = null!;
    private TextBox caesarKey = null!;
    private RichTextBox caesarOutput = null!;

    private TextBox vigenereFirst = null!;
    private TextBox vigenereSecond = null!;
    private TextBox vigenereKey = null!;
    private RichTextBox vigenereOutput = null!;

    private TextBox xorFirst = null!;
    private TextBox xorSecond = null!;
    private TextBox xorKey = null!;
    private RichTextBox xorOutput = null!;

    private TextBox rsaFirstWord = null!;
    private TextBox rsaSecondWord = null!;
    private TextBox rsaP1 = null!;
    private TextBox rsaQ1 = null!;
    private TextBox rsaE1 = null!;
    private TextBox rsaP2 = null!;
    private TextBox rsaQ2 = null!;
    private TextBox rsaE2 = null!;
    private RichTextBox rsaOutput = null!;

    private TextBox playfairFirstWord = null!;
    private TextBox playfairSecondWord = null!;
    private TextBox playfairFirstKey = null!;
    private TextBox playfairSecondKey = null!;
    private RichTextBox playfairOutput = null!;

    public MainForm()
    {
        Text = "Учебная программа по шифрованию";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 1250;
        Height = 820;
        MinimumSize = new Size(950, 650);
        AutoScaleMode = AutoScaleMode.Dpi;

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10)
        };

        tabs.TabPages.Add(CreateCaesarTab());
        tabs.TabPages.Add(CreateVigenereTab());
        tabs.TabPages.Add(CreateXorTab());
        tabs.TabPages.Add(CreateRsaTab());
        tabs.TabPages.Add(CreatePlayfairTab());
        tabs.TabPages.Add(CreateDemoTab());

        Controls.Add(tabs);
    }

    private TabPage CreateCaesarTab()
    {
        TabPage page = CreatePage("Цезарь", out FlowLayoutPanel inputs, out caesarOutput);
        caesarFirst = AddField(inputs, "Число Комаров-1", "1709", 130);
        caesarSecond = AddField(inputs, "Число Комаров-2", "482", 130);
        caesarKey = AddField(inputs, "Общий ключ (цифра 0..9)", "3", 140);
        AddRunButton(inputs, "Выполнить и создать PDF", RunCaesar);
        return page;
    }

    private TabPage CreateVigenereTab()
    {
        TabPage page = CreatePage("Виженер", out FlowLayoutPanel inputs, out vigenereOutput);
        vigenereFirst = AddField(inputs, "Слово Комаров-1", "ПРИВЕТ", 130);
        vigenereSecond = AddField(inputs, "Слово Комаров-2", "СТУДЕНТ", 130);
        vigenereKey = AddField(
            inputs,
            "Общая ключевая фраза (от 5 букв)",
            "МОЙ КЛЮЧ",
            180);
        AddRunButton(inputs, "Выполнить и создать PDF", RunVigenere);
        return page;
    }

    private TabPage CreateXorTab()
    {
        TabPage page = CreatePage("XOR", out FlowLayoutPanel inputs, out xorOutput);
        xorFirst = AddField(inputs, "Число Комаров-1", "25");
        xorSecond = AddField(inputs, "Число Комаров-2", "42");
        xorKey = AddField(inputs, "Общий ключ", "13");
        AddRunButton(inputs, "Выполнить и создать PDF", RunXor);
        return page;
    }

    private TabPage CreateRsaTab()
    {
        TabPage page = CreatePage("RSA", out FlowLayoutPanel inputs, out rsaOutput);
        rsaFirstWord = AddField(inputs, "Слово Комаров-1", "КОДЕР", 110);
        rsaSecondWord = AddField(inputs, "Слово Комаров-2", "ШИФР", 110);
        rsaP1 = AddField(inputs, "p1", "5", 60);
        rsaQ1 = AddField(inputs, "q1", "11", 60);
        rsaE1 = AddField(inputs, "e1", "3", 60);
        rsaP2 = AddField(inputs, "p2", "13", 60);
        rsaQ2 = AddField(inputs, "q2", "17", 60);
        rsaE2 = AddField(inputs, "e2", "5", 60);
        AddRunButton(inputs, "Выполнить и создать PDF", RunRsa);
        return page;
    }

    private TabPage CreatePlayfairTab()
    {
        TabPage page = CreatePage("Плейфер", out FlowLayoutPanel inputs, out playfairOutput);
        playfairFirstWord = AddField(inputs, "Слово Комаров-1", "HELLO", 120);
        playfairSecondWord = AddField(inputs, "Слово Комаров-2", "WORLD", 120);
        playfairFirstKey = AddField(inputs, "Ключ Комаров-1", "MONARCHY", 130);
        playfairSecondKey = AddField(inputs, "Ключ Комаров-2", "STUDENT", 130);
        AddRunButton(inputs, "Выполнить и создать PDF", RunPlayfair);
        return page;
    }

    private TabPage CreateDemoTab()
    {
        TabPage page = CreatePage("Демо", out FlowLayoutPanel inputs, out RichTextBox output);

        var description = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(670, 0),
            Text = "Запускает пять алгоритмов с подготовленными данными и создаёт " +
                   "пять отдельных PDF-отчётов."
        };
        inputs.Controls.Add(description);

        var button = new Button
        {
            AutoSize = true,
            Height = 36,
            Margin = new Padding(18, 18, 3, 3),
            Text = "Запустить все демонстрации"
        };
        button.Click += (_, _) => RunDemo(output);
        inputs.Controls.Add(button);
        return page;
    }

    private static TabPage CreatePage(
        string title,
        out FlowLayoutPanel inputs,
        out RichTextBox output)
    {
        var page = new TabPage(title)
        {
            Padding = new Padding(8)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        inputs = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(8),
            BackColor = Color.FromArgb(245, 247, 250)
        };

        output = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Consolas", 10),
            WordWrap = false,
            ScrollBars = RichTextBoxScrollBars.Both
        };

        layout.Controls.Add(inputs, 0, 0);
        layout.Controls.Add(output, 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    private static TextBox AddField(
        FlowLayoutPanel parent,
        string labelText,
        string value,
        int width = 90)
    {
        var group = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(4)
        };

        group.Controls.Add(new Label
        {
            AutoSize = true,
            Text = labelText
        });

        var textBox = new TextBox
        {
            Text = value,
            Width = width
        };
        group.Controls.Add(textBox);
        parent.Controls.Add(group);
        return textBox;
    }

    private static void AddRunButton(
        FlowLayoutPanel parent,
        string text,
        EventHandler clickHandler)
    {
        var button = new Button
        {
            AutoSize = true,
            Height = 36,
            Margin = new Padding(15, 22, 3, 3),
            Text = text
        };
        button.Click += clickHandler;
        parent.Controls.Add(button);
    }

    private void RunCaesar(object? sender, EventArgs e)
    {
        RunSafely(caesarOutput, () =>
        {
            List<string> lines = EncryptionScenarios.Caesar(
                caesarFirst.Text,
                caesarSecond.Text,
                ReadInt(caesarKey, "Общий ключ"));
            SaveAndShow("Цезарь", lines, caesarOutput);
        });
    }

    private void RunVigenere(object? sender, EventArgs e)
    {
        RunSafely(vigenereOutput, () =>
        {
            List<string> lines = EncryptionScenarios.Vigenere(
                vigenereFirst.Text,
                vigenereSecond.Text,
                vigenereKey.Text);
            SaveAndShow("Виженер", lines, vigenereOutput);
        });
    }

    private void RunXor(object? sender, EventArgs e)
    {
        RunSafely(xorOutput, () =>
        {
            List<string> lines = EncryptionScenarios.Xor(
                ReadInt(xorFirst, "Число Комаров-1"),
                ReadInt(xorSecond, "Число Комаров-2"),
                ReadInt(xorKey, "Общий ключ"));
            SaveAndShow("XOR", lines, xorOutput);
        });
    }

    private void RunRsa(object? sender, EventArgs e)
    {
        RunSafely(rsaOutput, () =>
        {
            List<string> lines = EncryptionScenarios.Rsa(
                rsaFirstWord.Text,
                rsaSecondWord.Text,
                ReadInt(rsaP1, "p1"),
                ReadInt(rsaQ1, "q1"),
                ReadInt(rsaE1, "e1"),
                ReadInt(rsaP2, "p2"),
                ReadInt(rsaQ2, "q2"),
                ReadInt(rsaE2, "e2"));
            SaveAndShow("RSA", lines, rsaOutput);
        });
    }

    private void RunPlayfair(object? sender, EventArgs e)
    {
        RunSafely(playfairOutput, () =>
        {
            List<string> lines = EncryptionScenarios.Playfair(
                playfairFirstWord.Text,
                playfairSecondWord.Text,
                playfairFirstKey.Text,
                playfairSecondKey.Text);
            SaveAndShow("Плейфер", lines, playfairOutput);
        });
    }

    private void RunDemo(RichTextBox output)
    {
        RunSafely(output, () =>
        {
            var createdFiles = new List<string>
            {
                pdfReportService.SaveReport(
                    "Цезарь",
                    EncryptionScenarios.Caesar("1709", "482", 3)),
                pdfReportService.SaveReport(
                    "Виженер",
                    EncryptionScenarios.Vigenere("ПРИВЕТ", "СТУДЕНТ", "МОЙ КЛЮЧ")),
                pdfReportService.SaveReport(
                    "XOR",
                    EncryptionScenarios.Xor(25, 42, 13)),
                pdfReportService.SaveReport(
                    "RSA",
                    EncryptionScenarios.Rsa("КОДЕР", "ШИФР", 5, 11, 3, 13, 17, 5)),
                pdfReportService.SaveReport(
                    "Плейфер",
                    EncryptionScenarios.Playfair("HELLO", "WORLD", "MONARCHY", "STUDENT"))
            };

            var lines = new List<string>
            {
                "Все пять демонстраций выполнены успешно.",
                "Созданы файлы:"
            };
            lines.AddRange(createdFiles.Select(path => $"- {path}"));
            output.Text = string.Join(Environment.NewLine, lines);
        });
    }

    private void SaveAndShow(
        string algorithmName,
        List<string> lines,
        RichTextBox output)
    {
        string reportPath = pdfReportService.SaveReport(algorithmName, lines);
        lines.Add("");
        lines.Add("PDF сохранён:");
        lines.Add(reportPath);
        output.Text = string.Join(Environment.NewLine, lines);
        output.SelectionStart = 0;
        output.ScrollToCaret();
    }

    private void RunSafely(RichTextBox output, Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            output.Text = "Ошибка:" + Environment.NewLine + exception.Message;
            MessageBox.Show(
                this,
                exception.Message,
                "Ошибка входных данных",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private static int ReadInt(TextBox textBox, string fieldName)
    {
        if (!int.TryParse(textBox.Text, out int value))
        {
            throw new ArgumentException($"Поле «{fieldName}» должно содержать целое число.");
        }

        return value;
    }
}
