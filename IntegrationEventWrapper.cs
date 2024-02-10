using GhostLyzer.Core.Domain.Event;

namespace GhostLyzer.Core.Domain
{
    /// <summary>
    /// Represents a wrapper for a domain event that implements the <see cref="IIntegrationEvent"/> interface.
    /// </summary>
    /// <typeparam name="TDomainEventType">The type of the domain event.</typeparam>
    public record IntegrationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) 
        : IIntegrationEvent where TDomainEventType : IDomainEvent;
}
