using GhostLyzer.Core.Domain.Event;

namespace GhostLyzer.Core.Domain
{
    /// <summary>
    /// Provides an interface for mapping domain events to integration events.
    /// </summary>
    public interface IEventMapper
    {
        /// <summary>
        /// Maps a single domain event to an integration event.
        /// </summary>
        /// <param name="event">The domain event to map.</param>
        /// <returns>The resulting integration event.</returns>
        IIntegrationEvent Map(IDomainEvent @event);

        /// <summary>
        /// Maps a collection of domain events to integration events.
        /// </summary>
        /// <param name="events">The domain events to map.</param>
        /// <returns>A collection of resulting integration events.</returns>
        IEnumerable<IIntegrationEvent> MapAll(IEnumerable<IDomainEvent> events);
    }
}
