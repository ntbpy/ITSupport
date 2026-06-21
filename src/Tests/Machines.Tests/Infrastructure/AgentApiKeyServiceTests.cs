using MIT.Modules.Machines.Infrastructure;
using Microsoft.Extensions.Options;

namespace Machines.Tests.Infrastructure;

public sealed class AgentApiKeyServiceTests
{
    private readonly AgentApiKeyService _sut = new(Options.Create(new AgentKeyOptions
    {
        EncryptionKey = "VietRMM-Test-32ByteEncryptionKey!!"
    }));

    [Fact]
    public void Generate_Returns32ByteBase64UrlKey()
    {
        var key = _sut.Generate();
        key.ShouldNotBeNullOrWhiteSpace();
        var normalized = key.Replace('-', '+').Replace('_', '/');
        normalized = normalized.PadRight(normalized.Length + (4 - normalized.Length % 4) % 4, '=');
        Convert.FromBase64String(normalized).Length.ShouldBe(32);
    }

    [Fact]
    public void EncryptDecrypt_RoundTrip()
    {
        var plaintext = _sut.Generate();
        var encrypted = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted);
        decrypted.ShouldBe(plaintext);
    }

    [Fact]
    public void Verify_CorrectKey_ReturnsTrue()
    {
        var plain = _sut.Generate();
        var encrypted = _sut.Encrypt(plain);
        _sut.Verify(plain, encrypted).ShouldBeTrue();
    }

    [Fact]
    public void Verify_WrongKey_ReturnsFalse()
    {
        var plain = _sut.Generate();
        var encrypted = _sut.Encrypt(plain);
        _sut.Verify("wrong-key", encrypted).ShouldBeFalse();
    }
}
