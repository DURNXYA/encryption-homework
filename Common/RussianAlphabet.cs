namespace EncryptionHomework.Common;

public static class RussianAlphabet
{
    public const string Letters =
        "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

    public static string NormalizeWord(string text, string fieldName)
    {
        string normalized = text.Trim().ToUpperInvariant();

        if (normalized.Length == 0)
        {
            throw new ArgumentException($"Поле «{fieldName}» не должно быть пустым.");
        }

        foreach (char letter in normalized)
        {
            if (!Letters.Contains(letter))
            {
                throw new ArgumentException(
                    $"Поле «{fieldName}» должно содержать только русские буквы.");
            }
        }

        return normalized;
    }

    public static int GetIndex(char letter)
    {
        int index = Letters.IndexOf(letter);

        if (index < 0)
        {
            throw new ArgumentException($"Буква «{letter}» отсутствует в русском алфавите.");
        }

        return index;
    }
}
