namespace EncryptionHomework.Models;

public class RsaKeyPair
{
    public int P { get; init; }

    public int Q { get; init; }

    public int N { get; init; }

    public int Phi { get; init; }

    public int E { get; init; }

    public int D { get; init; }

    public List<string> Steps { get; init; } = new();
}
