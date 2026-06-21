namespace MIT.Modules.Machines.Infrastructure;

public sealed class AgentKeyOptions
{
    public const string Section = "AgentKey";

    public string EncryptionKey { get; set; } = default!;  // exactly 32 ASCII chars
}
