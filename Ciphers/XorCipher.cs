using EncryptionHomework.Models;

namespace EncryptionHomework.Ciphers;

public static class XorCipher
{
    public static NumberCipherResult Transform(int value, int key)
    {
        if (value < 0 || key < 0)
        {
            throw new ArgumentException("Для учебной демонстрации используйте неотрицательные числа.");
        }

        int result = value ^ key;
        int width = Math.Max(ToBinary(value).Length, ToBinary(key).Length);
        width = Math.Max(width, ToBinary(result).Length);

        string valueBinary = ToBinary(value).PadLeft(width, '0');
        string keyBinary = ToBinary(key).PadLeft(width, '0');
        string resultBinary = ToBinary(result).PadLeft(width, '0');

        return new NumberCipherResult
        {
            Result = result,
            Steps =
            {
                $"Число: {value} = {valueBinary}",
                $"Ключ:  {key} = {keyBinary}",
                "Побитовое XOR:",
                $"  {valueBinary}",
                $"^ {keyBinary}",
                $"  {new string('-', width)}",
                $"  {resultBinary}",
                $"Результат: {result}"
            }
        };
    }

    private static string ToBinary(int value)
    {
        return Convert.ToString(value, 2);
    }
}
