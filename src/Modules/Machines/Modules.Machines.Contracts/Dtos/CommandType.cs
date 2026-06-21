namespace MIT.Modules.Machines.Contracts.Dtos;

public enum CommandType { InstallSoftware, PrepareRemote, RunScript, CheckUpdate, Fix }

public enum CommandStatus { Queued, Sent, Received, InProgress, Completed, Failed, PendingApproval }
