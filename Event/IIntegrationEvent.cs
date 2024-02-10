using MassTransit;

namespace GhostLyzer.Core.Domain.Event
{
    [ExcludeFromTopology]
    public interface IIntegrationEvent : IEvent
    {
    }
}
