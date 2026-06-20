using System.Collections.ObjectModel;
using MIT.Modules.Chat.Contracts.v1.DTOs;
using Mediator;

namespace MIT.Modules.Chat.Contracts.v1.Queries;

public sealed record DiscoverChannelsQuery(string? Search, int Page = 1, int PageSize = 50)
    : IQuery<ReadOnlyCollection<ChannelDto>>;
