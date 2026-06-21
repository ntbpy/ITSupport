using Mediator;

namespace MIT.Modules.Diagnostics.Contracts.v1.Diagnostics;

public sealed record AcknowledgeDiagnosticCommand(Guid ReportId) : ICommand;
