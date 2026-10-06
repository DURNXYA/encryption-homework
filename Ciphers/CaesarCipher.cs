using System.Text;
using EncryptionHomework.Models;

namespace EncryptionHomework.Ciphers;

public static class CaesarCipher
{
    public static CipherResult Encrypt(string number, int key)
    {
        return Transform(number, key, true);
    }

    public static CipherResult Decrypt(string number, int key)
    {
        return Transform(number, key, false);
    }

    private static CipherResult Transform(string number, int key, bool encrypt)
    {
        string normalizedNumber = NormalizeNumber(number);

        if (key < 0 || key > 9)
        {
            throw new ArgumentException("Ключ Цезаря должен быть одной цифрой от 0 до 9.");
        }

        var output = new StringBuilder();
        var result = new CipherResult();
        string operation = encrypt ? "+" : "-";

        result.Steps.Add(encrypt ? "Шифрование:" : "Расшифрование:");
        result.Steps.Add($"Число: {normalizedNumber}");
        result.Steps.Add($"Общий секретный ключ: {key}");

        for (int i = 0; i < normalizedNumber.Length; i++)
        {
            int digit = normalizedNumber[i] - '0';
            int transformedDigit = encrypt
                ? (digit + key) % 10
                : (digit - key + 10) % 10;

            output.Append(transformedDigit);
            result.Steps.Add(
                $"{i + 1}. ({digit} {operation} {key}" +
                $"{(encrypt ? "" : " + 10")}) mod 10 = {transformedDigit}");
        }

        result.Result = output.ToString();
        result.Steps.Add($"Результат: {result.Result}");
        return result;
    }

    private static string NormalizeNumber(string number)
    {
        string normalized = number.Trim();

        if (normalized.Length == 0)
        {
            throw new ArgumentException("Число для шифрования не должно быть пустым.");
        }

        foreach (char symbol in normalized)
        {
            if (symbol < '0' || symbol > '9')
            {
                throw new ArgumentException(
                    "Число для шифрования должно содержать только цифры от 0 до 9.");
            }
        }

        return normalized;
    }
}
