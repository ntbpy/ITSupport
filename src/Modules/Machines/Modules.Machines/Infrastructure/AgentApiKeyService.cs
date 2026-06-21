using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MIT.Modules.Machines.Infrastructure;

public interface IAgentApiKeyService
{
    string Generate();
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
    bool Verify(string plaintext, string encrypted);
}

public sealed class AgentApiKeyService(IOptions<AgentKeyOptions> options) : IAgentApiKeyService
{
    private readonly byte[] _key = Encoding.UTF8.GetBytes(options.Value.EncryptionKey)[..32];

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public string Encrypt(string plaintext)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor();
        var data = Encoding.UTF8.GetBytes(plaintext);
        var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
        var result = new byte[16 + encrypted.Length];
        aes.IV.CopyTo(result, 0);
        encrypted.CopyTo(result, 16);
        return Convert.ToBase64String(result);
    }

    public string Decrypt(string ciphertext)
    {
        var raw = Convert.FromBase64String(ciphertext);
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = raw[..16];
        using var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(raw, 16, raw.Length - 16);
        return Encoding.UTF8.GetString(decrypted);
    }

    public bool Verify(string plaintext, string encrypted)
    {
        try { return Decrypt(encrypted) == plaintext; }
        catch { return false; }
    }
}
