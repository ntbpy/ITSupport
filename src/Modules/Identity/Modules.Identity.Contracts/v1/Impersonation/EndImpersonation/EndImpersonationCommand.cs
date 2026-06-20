using MIT.Modules.Identity.Contracts.DTOs;
using Mediator;

namespace MIT.Modules.Identity.Contracts.v1.Impersonation.EndImpersonation;

public sealed record EndImpersonationCommand() : ICommand<TokenResponse>;
