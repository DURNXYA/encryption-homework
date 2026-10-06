using EncryptionHomework.Ciphers;
using EncryptionHomework.Common;

namespace EncryptionHomework.Application;

public static class EncryptionScenarios
{
    public static List<string> Caesar(string firstNumber, string secondNumber, int key)
    {
        var lines = CreateHeader("ШИФР ЦЕЗАРЯ");
        lines.Add($"Общий секретный ключ: {key}");
        lines.Add("Алфавит: десятичные цифры 0 1 2 3 4 5 6 7 8 9");
        lines.Add("Каждая цифра числа сдвигается отдельно по модулю 10.");

        AddSection(lines, "Комаров-1 -> Комаров-2");
        AddCaesarDirection(lines, firstNumber, key);

        AddSection(lines, "Комаров-2 -> Комаров-1");
        AddCaesarDirection(lines, secondNumber, key);
        return lines;
    }

    public static List<string> Vigenere(string firstWord, string secondWord, string key)
    {
        var lines = CreateHeader("ШИФР ВИЖЕНЕРА");
        lines.Add($"Введённая ключевая фраза: {key.Trim().ToUpperInvariant()}");
        lines.Add($"Ключ без пробелов: {VigenereCipher.NormalizeKey(key)}");
        lines.Add($"Алфавит: {RussianAlphabet.Letters}");
        lines.Add("Вместо квадрата Виженера используются вычисления по модулю 33.");

        AddSection(lines, "Комаров-1 -> Комаров-2");
        AddVigenereDirection(lines, firstWord, key);

        AddSection(lines, "Комаров-2 -> Комаров-1");
        AddVigenereDirection(lines, secondWord, key);
        return lines;
    }

    public static List<string> Xor(int firstValue, int secondValue, int key)
    {
        var lines = CreateHeader("ШИФРОВАНИЕ XOR");
        lines.Add($"Общий секретный ключ: {key}");

        AddSection(lines, "Комаров-1 -> Комаров-2");
        AddXorDirection(lines, firstValue, key);

        AddSection(lines, "Комаров-2 -> Комаров-1");
        AddXorDirection(lines, secondValue, key);
        return lines;
    }

    public static List<string> Rsa(
        string firstWord,
        string secondWord,
        int p1,
        int q1,
        int e1,
        int p2,
        int q2,
        int e2)
    {
        var lines = CreateHeader("ШИФРОВАНИЕ RSA");
        lines.Add($"Алфавит: {RussianAlphabet.Letters}");
        lines.Add("Кодирование букв: А = 1, Б = 2, ..., Я = 33");

        var firstKey = RsaCipher.CreateKeyPair(p1, q1, e1);
        var secondKey = RsaCipher.CreateKeyPair(p2, q2, e2);

        if (firstKey.N == secondKey.N && firstKey.E == secondKey.E)
        {
            throw new ArgumentException("У участников должны быть разные пары ключей RSA.");
        }

        AddSection(lines, "Ключи Комаров-1");
        lines.AddRange(firstKey.Steps);
        AddSection(lines, "Ключи Комаров-2");
        lines.AddRange(secondKey.Steps);

        AddSection(lines, "Комаров-1 -> Комаров-2");
        var firstEncrypted = RsaCipher.Encrypt(firstWord, secondKey);
        lines.AddRange(firstEncrypted.Steps);
        lines.Add("");
        var firstDecrypted = RsaCipher.Decrypt(firstEncrypted.EncryptedNumbers, secondKey);
        lines.AddRange(firstDecrypted.Steps);
        AddCheck(
            lines,
            RussianAlphabet.NormalizeWord(firstWord, "Первое слово"),
            firstDecrypted.Result);

        AddSection(lines, "Комаров-2 -> Комаров-1");
        var secondEncrypted = RsaCipher.Encrypt(secondWord, firstKey);
        lines.AddRange(secondEncrypted.Steps);
        lines.Add("");
        var secondDecrypted = RsaCipher.Decrypt(secondEncrypted.EncryptedNumbers, firstKey);
        lines.AddRange(secondDecrypted.Steps);
        AddCheck(
            lines,
            RussianAlphabet.NormalizeWord(secondWord, "Второе слово"),
            secondDecrypted.Result);
        return lines;
    }

    public static List<string> Playfair(
        string firstWord,
        string secondWord,
        string firstKey,
        string secondKey)
    {
        var lines = CreateHeader("ШИФР ПЛЕЙФЕРА");
        lines.Add("Используется английский алфавит 5x5; буквы I и J объединены.");
        lines.Add("Каждый отправитель шифрует сообщение ключом получателя.");

        AddSection(lines, "Ключ и матрица Комаров-1");
        lines.Add($"Ключ: {firstKey.Trim().ToUpperInvariant()}");
        lines.AddRange(PlayfairCipher.FormatMatrix(PlayfairCipher.BuildMatrix(firstKey)));

        AddSection(lines, "Ключ и матрица Комаров-2");
        lines.Add($"Ключ: {secondKey.Trim().ToUpperInvariant()}");
        lines.AddRange(PlayfairCipher.FormatMatrix(PlayfairCipher.BuildMatrix(secondKey)));

        AddSection(lines, "Комаров-1 -> Комаров-2");
        AddPlayfairDirection(lines, firstWord, secondKey, "Комаров-2");

        AddSection(lines, "Комаров-2 -> Комаров-1");
        AddPlayfairDirection(lines, secondWord, firstKey, "Комаров-1");
        return lines;
    }

    private static void AddVigenereDirection(List<string> lines, string word, string key)
    {
        var encrypted = VigenereCipher.Encrypt(word, key);
        lines.AddRange(encrypted.Steps);
        lines.Add("");
        var decrypted = VigenereCipher.Decrypt(encrypted.Result, key);
        lines.AddRange(decrypted.Steps);
        AddCheck(
            lines,
            RussianAlphabet.NormalizeWord(word, "Слово"),
            decrypted.Result);
    }

    private static void AddCaesarDirection(List<string> lines, string number, int key)
    {
        string normalizedNumber = number.Trim();
        var encrypted = CaesarCipher.Encrypt(normalizedNumber, key);
        lines.AddRange(encrypted.Steps);
        lines.Add("");
        var decrypted = CaesarCipher.Decrypt(encrypted.Result, key);
        lines.AddRange(decrypted.Steps);
        AddCheck(lines, normalizedNumber, decrypted.Result);
    }

    private static void AddXorDirection(List<string> lines, int value, int key)
    {
        var encrypted = XorCipher.Transform(value, key);
        lines.Add("Шифрование:");
        lines.AddRange(encrypted.Steps);
        lines.Add("");
        var decrypted = XorCipher.Transform(encrypted.Result, key);
        lines.Add("Повторный XOR для расшифрования:");
        lines.AddRange(decrypted.Steps);
        AddCheck(lines, value.ToString(), decrypted.Result.ToString());
    }

    private static void AddPlayfairDirection(
        List<string> lines,
        string word,
        string recipientKey,
        string recipientName)
    {
        string prepared = PlayfairCipher.PrepareText(word);
        lines.Add($"Исходный текст: {word.Trim().ToUpperInvariant()}");
        lines.Add($"Подготовленный текст: {prepared}");
        lines.Add($"Используется ключ получателя {recipientName}: " +
                  recipientKey.Trim().ToUpperInvariant());

        var encrypted = PlayfairCipher.Encrypt(word, recipientKey);
        lines.AddRange(encrypted.Steps);
        lines.Add("");
        var decrypted = PlayfairCipher.Decrypt(encrypted.Result, recipientKey);
        lines.AddRange(decrypted.Steps);
        lines.Add("Плейфер проверяется относительно подготовленного текста.");
        AddCheck(lines, prepared, decrypted.Result);
    }

    private static List<string> CreateHeader(string algorithm)
    {
        return new List<string>
        {
            algorithm,
            "Участник 1: Комаров-1",
            "Участник 2: Комаров-2",
            ""
        };
    }

    private static void AddSection(List<string> lines, string title)
    {
        lines.Add("");
        lines.Add(new string('=', 60));
        lines.Add(title);
        lines.Add(new string('=', 60));
        lines.Add("");
    }

    private static void AddCheck(List<string> lines, string expected, string actual)
    {
        bool success = expected == actual;
        lines.Add("");
        lines.Add("Проверка:");
        lines.Add($"{expected} == {actual}");
        lines.Add(success ? "Результат: УСПЕШНО" : "Результат: ОШИБКА");

        if (!success)
        {
            throw new InvalidOperationException("Результат не совпал с исходным значением.");
        }
    }
}
