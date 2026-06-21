using Mediator;

namespace MIT.Modules.Alerts.Contracts.v1.Alerts;

public sealed record AcknowledgeAlertCommand(Guid AlertId) : ICommand;
