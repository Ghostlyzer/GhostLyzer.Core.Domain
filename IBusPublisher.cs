using GhostLyzer.Core.Domain.Event;

namespace GhostLyzer.Core.Domain
{
    /// <summary>
    /// Represents a service for publishing events to a bus.
    /// </summary>
    public interface IBusPublisher
    {
        /// <summary>
        /// Sends a domain event asynchronously to the bus.
        /// </summary>
        /// <param name="domainEvent">The domain event to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a list of domain events asynchronously to the bus.
        /// </summary>
        /// <param name="domainEvents">The list of domain events to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an integration event asynchronously to the bus.
        /// </summary>
        /// <param name="integrationEvent">The integration event to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a list of integration events asynchronously to the bus.
        /// </summary>
        /// <param name="integrationEvents">The list of integration events to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SendAsync(IReadOnlyList<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default);
    }
}
