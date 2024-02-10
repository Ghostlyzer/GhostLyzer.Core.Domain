using GhostLyzer.Core.Domain.Event;
using GhostLyzer.Core.Domain.Models;

namespace GhostLyzer.Core.Domain.Model
{
    /// <summary>
    /// Represents an aggregate with a long identifier.
    /// </summary>
    public abstract class Aggregate : Aggregate<long>
    {
    }

    /// <summary>
    /// Represents an aggregate with a generic identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public abstract class Aggregate<TId> : Entity, IAggregate<TId>
    {
        /// <summary>
        /// The list of domain events associated with the aggregate.
        /// </summary>
        private readonly List<IDomainEvent> _domainEvents = [];

        /// <summary>
        /// Gets or sets the version of the aggregate.
        /// </summary>
        public long Version { get; set; } = -1;

        /// <summary>
        /// Gets the identifier of the aggregate.
        /// </summary>
        public TId Id { get; protected set; }

        /// <summary>
        /// Gets the read-only list of domain events associated with the aggregate.
        /// </summary>
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Adds a domain event to the aggregate.
        /// </summary>
        /// <param name="domainEvent">The domain event to add.</param>
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clears the domain events from the aggregate and returns them.
        /// </summary>
        /// <returns>The domain events that were cleared from the aggregate.</returns>
        public IEvent[] ClearDomainEvents()
        {
            IEvent[] dequeuedEvents = _domainEvents.ToArray();

            _domainEvents.Clear();

            return dequeuedEvents;
        }
    }
}
