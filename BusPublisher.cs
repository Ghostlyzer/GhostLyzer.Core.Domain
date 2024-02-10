using DotNetCore.CAP;
using GhostLyzer.Core.Domain.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GhostLyzer.Core.Domain
{

    /// <summary>
    /// Represents a publisher that sends domain and integration events to a bus.
    /// </summary>
    public sealed class BusPublisher : IBusPublisher
    {
        private readonly IEventMapper _eventMapper;
        private readonly ILogger<BusPublisher> _logger;
        private readonly ICapPublisher _capPublisher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusPublisher"/> class.
        /// </summary>
        /// <param name="eventMapper">The event mapper.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="capPublisher">The CAP publisher.</param>
        /// <param name="serviceScopeFactory">The service scope factory.</param>
        public BusPublisher(
            IEventMapper eventMapper,
            ILogger<BusPublisher> logger,
            ICapPublisher capPublisher,
            IServiceScopeFactory serviceScopeFactory)
        {
            _eventMapper = eventMapper;
            _logger = logger;
            _capPublisher = capPublisher;
            _serviceScopeFactory = serviceScopeFactory;
        }


        /// <summary>
        /// Sends a domain event asynchronously to the bus.
        /// </summary>
        /// <param name="domainEvent">The domain event to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendAsync(IDomainEvent domainEvent,
            CancellationToken cancellationToken = default) => await SendAsync(new[] { domainEvent }, cancellationToken);

        /// <summary>
        /// Sends a list of domain events asynchronously to the bus.
        /// </summary>
        /// <param name="domainEvents">The list of domain events to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            if (domainEvents is null)
            {
                _logger.LogWarning("Bus Publisher: No domain events to process.");
                return;
            }

            _logger.LogTrace("Bus Publisher: Starting to process domain events...");

            var integrationEvents = await MapDomainEventToIntegrationEventAsync(domainEvents).ConfigureAwait(false);

            if (!integrationEvents.Any())
            {
                _logger.LogWarning("Bus Publisher: No integration events to process after mapping.");
                return;
            }

            foreach (var integrationEvent in integrationEvents)
            {
                try
                {
                    await _capPublisher.PublishAsync(integrationEvent.GetType().Name, integrationEvent, cancellationToken: cancellationToken);
                    _logger.LogTrace("Bus Publisher: Published a message with ID {Id}", integrationEvent?.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Bus Publisher: Error publishing integration event with ID {Id}", integrationEvent?.EventId);
                }
            }

            _logger.LogTrace("Bus Publisher: Done processing domain events...");
        }

        /// <summary>
        /// Sends an integration event asynchronously to the bus.
        /// </summary>
        /// <param name="integrationEvent">The integration event to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendAsync(IIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default) => await SendAsync(new[] { integrationEvent }, cancellationToken);

        /// <summary>
        /// Sends a list of integration events asynchronously to the bus.
        /// </summary>
        /// <param name="integrationEvents">The list of integration events to send.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendAsync(IReadOnlyList<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default)
        {
            if (integrationEvents is null)
            {
                _logger.LogWarning("Bus Publisher: No integration events to process.");
                return;
            }

            _logger.LogTrace("Bus Publisher: Starting to process integration events...");

            foreach (var integrationEvent in integrationEvents)
            {
                try
                {
                    await _capPublisher.PublishAsync(integrationEvent.GetType().Name, integrationEvent, cancellationToken: cancellationToken);
                    _logger.LogTrace("Bus Publisher: Published a message with ID {Id}", integrationEvent?.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Bus Publisher: Error publishing integration event with ID {Id}", integrationEvent?.EventId);
                }
            }

            _logger.LogTrace("Bus Publisher: Done processing integration events...");
        }

        #region Helpers

        /// <summary>
        /// Maps a list of domain events to integration events.
        /// </summary>
        /// <param name="domainEvents">The list of domain events to map.</param>
        /// <returns>A task that represents the asynchronous operation and contains the list of integration events.</returns>
        private Task<IReadOnlyList<IIntegrationEvent>> MapDomainEventToIntegrationEventAsync(IReadOnlyList<IDomainEvent> domainEvents)
        {
            if (domainEvents == null)
            {
                throw new ArgumentNullException(nameof(domainEvents));
            }

            var wrappedIntegrationEvents = GetWrappedIntegrationEvents(domainEvents).ToList();

            if (wrappedIntegrationEvents.Count > 0)
            {
                return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(wrappedIntegrationEvents);
            }

            var integrationEvents = new List<IIntegrationEvent>();
            using var scope = _serviceScopeFactory.CreateScope();

            foreach (var @event in domainEvents)
            {
                var eventType = @event.GetType();
                _logger.LogTrace($"Handling Domain Event: {eventType.Name}");

                var integrationEvent = _eventMapper.Map(@event);

                if (integrationEvent != null)
                {
                    integrationEvents.Add(integrationEvent);
                }
            }

            return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(integrationEvents);
        }

        /// <summary>
        /// Wraps a list of domain events in integration events.
        /// </summary>
        /// <param name="domainEvents">The list of domain events to wrap.</param>
        /// <returns>The list of wrapped integration events.</returns>
        private IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
        {
            if (domainEvents == null)
            {
                throw new ArgumentNullException(nameof(domainEvents));
            }

            // Iterate over each domain event that implements IHaveIntegrationEvent
            foreach (var domainEvent in domainEvents.Where(x => x is IHaveIntegrationEvent))
            {
                // Create a generic type for the IntegrationEventWrapper based on the type of the domain event
                var genericType = typeof(IntegrationEventWrapper<>).MakeGenericType(domainEvent.GetType());

                // Try to create an instance of the generic type and check if it's an IIntegrationEvent
                if (Activator.CreateInstance(genericType, domainEvent) is IIntegrationEvent domainNotificationEvent)
                {
                    yield return domainNotificationEvent;
                }
                else
                {
                    _logger.LogError($"Failed to create instance of type {genericType} for domain event {domainEvent.GetType()}");
                }
            }
        }

        #endregion
    }
}
