using System.Text;
using EncryptionHomework.Models;

namespace EncryptionHomework.Ciphers;

public static class PlayfairCipher
{
    private const string Alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";

    public static CipherResult Encrypt(string text, string key)
    {
        char[,] matrix = BuildMatrix(key);
        string prepared = PrepareText(text);
        return Transform(prepared, matrix, true);
    }

    public static CipherResult Decrypt(string text, string key)
    {
        char[,] matrix = BuildMatrix(key);
        string normalized = NormalizeLetters(text, "Шифротекст Плейфера");

        if (normalized.Length % 2 != 0)
        {
            throw new ArgumentException("Шифротекст Плейфера должен иметь чётную длину.");
        }

        return Transform(normalized, matrix, false);
    }

    public static string PrepareText(string text)
    {
        string normalized = NormalizeLetters(text, "Текст Плейфера");
        var prepared = new StringBuilder();
        int index = 0;

        while (index < normalized.Length)
        {
            char first = normalized[index];

            if (index + 1 >= normalized.Length)
            {
                prepared.Append(first);
                prepared.Append(GetFiller(first));
                index++;
                continue;
            }

            char second = normalized[index + 1];

            if (first == second)
            {
                prepared.Append(first);
                prepared.Append(GetFiller(first));
                index++;
            }
            else
            {
                prepared.Append(first);
                prepared.Append(second);
                index += 2;
            }
        }

        return prepared.ToString();
    }

    public static char[,] BuildMatrix(string key)
    {
        string normalizedKey = NormalizeLetters(key, "Ключ Плейфера");
        var unique = new StringBuilder();

        foreach (char letter in normalizedKey + Alphabet)
        {
            if (!unique.ToString().Contains(letter))
            {
                unique.Append(letter);
            }
        }

        var matrix = new char[5, 5];

        for (int i = 0; i < 25; i++)
        {
            matrix[i / 5, i % 5] = unique[i];
        }

        return matrix;
    }

    public static List<string> FormatMatrix(char[,] matrix)
    {
        var lines = new List<string>();

        for (int row = 0; row < 5; row++)
        {
            var letters = new char[5];

            for (int column = 0; column < 5; column++)
            {
                letters[column] = matrix[row, column];
            }

            lines.Add(string.Join("  ", letters));
        }

        return lines;
    }

    private static CipherResult Transform(string text, char[,] matrix, bool encrypt)
    {
        var result = new CipherResult();
        var output = new StringBuilder();
        int direction = encrypt ? 1 : -1;

        result.Steps.Add(encrypt ? "Шифрование биграмм:" : "Расшифрование биграмм:");

        for (int i = 0; i < text.Length; i += 2)
        {
            char first = text[i];
            char second = text[i + 1];
            (int firstRow, int firstColumn) = FindPosition(matrix, first);
            (int secondRow, int secondColumn) = FindPosition(matrix, second);
            char resultFirst;
            char resultSecond;
            string rule;

            if (firstRow == secondRow)
            {
                resultFirst = matrix[firstRow, Wrap(firstColumn + direction)];
                resultSecond = matrix[secondRow, Wrap(secondColumn + direction)];
                rule = encrypt ? "одна строка, сдвиг вправо" : "одна строка, сдвиг влево";
            }
            else if (firstColumn == secondColumn)
            {
                resultFirst = matrix[Wrap(firstRow + direction), firstColumn];
                resultSecond = matrix[Wrap(secondRow + direction), secondColumn];
                rule = encrypt ? "один столбец, сдвиг вниз" : "один столбец, сдвиг вверх";
            }
            else
            {
                resultFirst = matrix[firstRow, secondColumn];
                resultSecond = matrix[secondRow, firstColumn];
                rule = "прямоугольник, меняем столбцы";
            }

            output.Append(resultFirst);
            output.Append(resultSecond);
            result.Steps.Add(
                $"{first}{second} -> {resultFirst}{resultSecond}: {rule}");
        }

        result.Result = output.ToString();
        result.Steps.Add($"Результат: {result.Result}");
        return result;
    }

    private static string NormalizeLetters(string text, string fieldName)
    {
        string normalized = text.Replace(" ", "").Trim().ToUpperInvariant().Replace('J', 'I');

        if (normalized.Length == 0)
        {
            throw new ArgumentException($"Поле «{fieldName}» не должно быть пустым.");
        }

        foreach (char letter in normalized)
        {
            if (letter < 'A' || letter > 'Z')
            {
                throw new ArgumentException(
                    $"Поле «{fieldName}» должно содержать только английские буквы.");
            }
        }

        return normalized;
    }

    private static (int Row, int Column) FindPosition(char[,] matrix, char letter)
    {
        for (int row = 0; row < 5; row++)
        {
            for (int column = 0; column < 5; column++)
            {
                if (matrix[row, column] == letter)
                {
                    return (row, column);
                }
            }
        }

        throw new InvalidOperationException($"Буква {letter} не найдена в матрице.");
    }

    private static int Wrap(int value)
    {
        return (value + 5) % 5;
    }

    private static char GetFiller(char letter)
    {
        return letter == 'X' ? 'Q' : 'X';
    }
}
