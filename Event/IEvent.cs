using MediatR;

namespace GhostLyzer.Core.Domain.Event
{
    /// <summary>
    /// Represents a domain event that can be handled by the MediatR library.
    /// </summary>
    public interface IEvent : INotification
    {
        /// <summary>
        /// Gets the unique identifier for the event.
        /// </summary>
        Guid EventId => Guid.NewGuid();

        /// <summary>
        /// Gets the date and time when the event occurred.
        /// </summary>
        DateTime OccuredOn => DateTime.Now;

        /// <summary>
        /// Gets the fully qualified name of the event type.
        /// </summary>
        string EventType => GetType().AssemblyQualifiedName;
    }
}
