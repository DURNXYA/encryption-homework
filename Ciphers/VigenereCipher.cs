using System.Text;
using EncryptionHomework.Common;
using EncryptionHomework.Models;

namespace EncryptionHomework.Ciphers;

public static class VigenereCipher
{
    public static CipherResult Encrypt(string text, string key)
    {
        return Transform(text, key, true);
    }

    public static CipherResult Decrypt(string text, string key)
    {
        return Transform(text, key, false);
    }

    private static CipherResult Transform(string text, string key, bool encrypt)
    {
        string normalizedText = RussianAlphabet.NormalizeWord(text, "Текст");
        string normalizedKey = NormalizeKey(key);

        if (normalizedKey.Length < 5)
        {
            throw new ArgumentException("Ключ Виженера должен содержать не менее 5 букв.");
        }

        var result = new CipherResult();
        var output = new char[normalizedText.Length];
        string operation = encrypt ? "+" : "-";

        result.Steps.Add(encrypt ? "Шифрование:" : "Расшифрование:");
        result.Steps.Add($"Текст: {normalizedText}");
        result.Steps.Add($"Ключ: {normalizedKey}");

        for (int i = 0; i < normalizedText.Length; i++)
        {
            char textLetter = normalizedText[i];
            char keyLetter = normalizedKey[i % normalizedKey.Length];
            int textIndex = RussianAlphabet.GetIndex(textLetter);
            int keyIndex = RussianAlphabet.GetIndex(keyLetter);
            int resultIndex = encrypt
                ? (textIndex + keyIndex) % RussianAlphabet.Letters.Length
                : (textIndex - keyIndex + RussianAlphabet.Letters.Length)
                    % RussianAlphabet.Letters.Length;

            output[i] = RussianAlphabet.Letters[resultIndex];
            result.Steps.Add(
                $"{i + 1}. {textLetter}({textIndex}) {operation} " +
                $"{keyLetter}({keyIndex}) mod 33 = {output[i]}({resultIndex})");
        }

        result.Result = new string(output);
        result.Steps.Add($"Результат: {result.Result}");
        return result;
    }

    public static string NormalizeKey(string key)
    {
        var lettersWithoutSpaces = new StringBuilder();

        foreach (char symbol in key)
        {
            if (!char.IsWhiteSpace(symbol))
            {
                lettersWithoutSpaces.Append(symbol);
            }
        }

        return RussianAlphabet.NormalizeWord(
            lettersWithoutSpaces.ToString(),
            "Ключ Виженера");
    }
}
