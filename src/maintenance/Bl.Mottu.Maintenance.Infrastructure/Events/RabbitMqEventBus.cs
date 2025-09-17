using Bl.Mottu.Maintenance.Core.Events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Bl.Mottu.Maintenance.Infrastructure.Events;
internal class RabbitMqEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqEventBus> _logger;

    public RabbitMqEventBus(IConnection connection, ILogger<RabbitMqEventBus> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IntegrationEvent
    {
        try
        {
            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "site-events",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();

            properties.Persistent = true;
            properties.Headers = @event.Filters.ToDictionary();

            channel.BasicPublish(
                exchange: "site-events",
                routingKey: @event.GetType().Name,
                mandatory: true,
                basicProperties: properties,
                body: body);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // Log error and potentially implement retry logic
            _logger.LogError(ex, "Failed to publish event {EventType}", @event.GetType().Name);
            throw;
        }
    }
}
