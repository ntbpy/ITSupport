using MIT.Modules.Alerts.Infrastructure;
using NSubstitute;
using StackExchange.Redis;

namespace Alerts.Tests.Infrastructure;

public sealed class AlertDeduplicationServiceTests
{
    [Fact]
    public async Task ShouldSend_NoPriorAlert_ReturnsTrue()
    {
        var db = Substitute.For<IDatabase>();
        db.StringSetAsync(
            Arg.Any<RedisKey>(), Arg.Any<RedisValue>(),
            Arg.Any<TimeSpan?>(), Arg.Any<bool>(), When.NotExists, Arg.Any<CommandFlags>())
            .Returns(true);
        var redis = Substitute.For<IConnectionMultiplexer>();
        redis.GetDatabase(Arg.Any<int>(), Arg.Any<object?>()).Returns(db);

        var sut = new AlertDeduplicationService(redis);
        var result = await sut.ShouldSendAsync(Guid.NewGuid(), Guid.NewGuid(), "CPU_HIGH");

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldSend_RecentAlert_ReturnsFalse()
    {
        var db = Substitute.For<IDatabase>();
        db.StringSetAsync(
            Arg.Any<RedisKey>(), Arg.Any<RedisValue>(),
            Arg.Any<TimeSpan?>(), Arg.Any<bool>(), When.NotExists, Arg.Any<CommandFlags>())
            .Returns(false);
        var redis = Substitute.For<IConnectionMultiplexer>();
        redis.GetDatabase(Arg.Any<int>(), Arg.Any<object?>()).Returns(db);

        var sut = new AlertDeduplicationService(redis);
        var result = await sut.ShouldSendAsync(Guid.NewGuid(), Guid.NewGuid(), "CPU_HIGH");

        result.ShouldBeFalse();
    }
}
