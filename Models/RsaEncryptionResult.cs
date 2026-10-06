namespace EncryptionHomework.Models;

public class RsaEncryptionResult
{
    public List<long> EncryptedNumbers { get; set; } = new();

    public List<string> Steps { get; set; } = new();
}
