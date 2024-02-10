using GhostLyzer.Core.Domain.Event;

namespace GhostLyzer.Core.Domain.Model
{
    /// <summary>
    /// Represents an aggregate entity in the domain-driven design context.
    /// </summary>
    public interface IAggregate : IEntity
    {
        /// <summary>
        /// Gets the read-only list of domain events associated with the aggregate.
        /// </summary>
        IReadOnlyList<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Clears the domain events from the aggregate and returns them.
        /// </summary>
        /// <returns>The domain events that were cleared from the aggregate.</returns>
        IEvent[] ClearDomainEvents();

        /// <summary>
        /// Gets or sets the version of the aggregate.
        /// </summary>
        long Version { get; set; }
    }

    /// <summary>
    /// Represents an aggregate entity with a specific type of identifier.
    /// </summary>
    /// <typeparam name="T">The type of the identifier.</typeparam>
    public interface IAggregate<out T> : IAggregate
    {
        /// <summary>
        /// Gets the identifier of the aggregate.
        /// </summary>
        T Id { get; }
    }
}
