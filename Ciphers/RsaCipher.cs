using EncryptionHomework.Common;
using EncryptionHomework.Models;

namespace EncryptionHomework.Ciphers;

public static class RsaCipher
{
    public static RsaKeyPair CreateKeyPair(int p, int q, int e)
    {
        if (!IsPrime(p) || !IsPrime(q))
        {
            throw new ArgumentException("p и q должны быть простыми числами.");
        }

        if (p == q)
        {
            throw new ArgumentException("Для наглядности p и q должны различаться.");
        }

        if (p > 1000 || q > 1000)
        {
            throw new ArgumentException("Для учебного примера используйте p и q не больше 1000.");
        }

        int n = p * q;
        int phi = (p - 1) * (q - 1);

        if (n <= RussianAlphabet.Letters.Length)
        {
            throw new ArgumentException("N должно быть больше 33 для русского алфавита.");
        }

        if (e <= 1 || e >= phi)
        {
            throw new ArgumentException("e должно быть больше 1 и меньше φ(N).");
        }

        int gcd = Gcd(e, phi);

        if (gcd != 1)
        {
            throw new ArgumentException("e и φ(N) должны быть взаимно простыми.");
        }

        int d = FindPrivateExponent(e, phi);

        return new RsaKeyPair
        {
            P = p,
            Q = q,
            N = n,
            Phi = phi,
            E = e,
            D = d,
            Steps =
            {
                $"p = {p}, простое число",
                $"q = {q}, простое число",
                $"N = p * q = {p} * {q} = {n}",
                $"φ(N) = (p - 1) * (q - 1) = {p - 1} * {q - 1} = {phi}",
                $"e = {e}",
                $"НОД({e}, {phi}) = {gcd}",
                $"Ищем d: ({e} * d) mod {phi} = 1",
                $"d = {d}, потому что ({e} * {d}) mod {phi} = 1",
                $"Открытый ключ: ({e}, {n})",
                $"Закрытый ключ: ({d}, {n})"
            }
        };
    }

    public static RsaEncryptionResult Encrypt(string text, RsaKeyPair recipientKey)
    {
        string normalized = RussianAlphabet.NormalizeWord(text, "Сообщение RSA");

        if (normalized.Length < 4 || normalized.Length > 6)
        {
            throw new ArgumentException("Сообщение RSA должно содержать от 4 до 6 букв.");
        }

        var result = new RsaEncryptionResult();
        result.Steps.Add($"Исходное слово: {normalized}");
        result.Steps.Add(
            $"Используем открытый ключ получателя: ({recipientKey.E}, {recipientKey.N})");

        foreach (char letter in normalized)
        {
            int number = RussianAlphabet.GetIndex(letter) + 1;
            long encrypted = ModPow(number, recipientKey.E, recipientKey.N);
            result.EncryptedNumbers.Add(encrypted);
            result.Steps.Add(
                $"{letter} -> {number}; {number}^{recipientKey.E} mod " +
                $"{recipientKey.N} = {encrypted}");
        }

        result.Steps.Add(
            $"Шифротекст: {string.Join(" ", result.EncryptedNumbers)}");
        return result;
    }

    public static CipherResult Decrypt(
        IEnumerable<long> encryptedNumbers,
        RsaKeyPair ownerKey)
    {
        var result = new CipherResult();
        var letters = new List<char>();

        result.Steps.Add(
            $"Используем закрытый ключ владельца: ({ownerKey.D}, {ownerKey.N})");

        foreach (long encrypted in encryptedNumbers)
        {
            long number = ModPow(encrypted, ownerKey.D, ownerKey.N);

            if (number < 1 || number > RussianAlphabet.Letters.Length)
            {
                throw new InvalidOperationException(
                    $"После расшифрования получено недопустимое число {number}.");
            }

            char letter = RussianAlphabet.Letters[(int)number - 1];
            letters.Add(letter);
            result.Steps.Add(
                $"{encrypted}^{ownerKey.D} mod {ownerKey.N} = {number} -> {letter}");
        }

        result.Result = new string(letters.ToArray());
        result.Steps.Add($"Расшифрованное слово: {result.Result}");
        return result;
    }

    private static bool IsPrime(int number)
    {
        if (number < 2)
        {
            return false;
        }

        for (int divisor = 2; divisor * divisor <= number; divisor++)
        {
            if (number % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int remainder = a % b;
            a = b;
            b = remainder;
        }

        return a;
    }

    private static int FindPrivateExponent(int e, int phi)
    {
        for (int d = 1; d < phi; d++)
        {
            if ((long)d * e % phi == 1)
            {
                return d;
            }
        }

        throw new InvalidOperationException("Не удалось найти закрытую экспоненту d.");
    }

    private static long ModPow(long value, int power, long modulus)
    {
        long result = 1;

        for (int i = 0; i < power; i++)
        {
            result = result * value % modulus;
        }

        return result;
    }
}
