using Mediator;

namespace MIT.Modules.Identity.Contracts.v1.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(Guid SessionId) : ICommand<bool>;