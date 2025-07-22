using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StockManagementAPI.DTOs.Messages;
using StockManagementAPI.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace StockManagementAPI.Services.Implementations
{
    public class RabbitMQService : IMessageService, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly string _exchangeName = "book_inventory";
        private readonly ILogger<RabbitMQService> _logger;
        private readonly RabbitMQOptions _options;
        private bool _isConnected = false;

        public RabbitMQService(IOptions<RabbitMQOptions> options, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            _options = options.Value;

            TryConnect();
        }

        private void TryConnect()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    VirtualHost = _options.VirtualHost,
                    Port = _options.Port,
                    // Add timeout to avoid long waits on connection failure
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
                };

                _logger.LogInformation($"Attempting to connect to RabbitMQ at {_options.HostName}:{_options.Port}");

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Configure exchange and queue
                _channel.ExchangeDeclare(
                    exchange: _exchangeName,
                    type: "direct",
                    durable: true,
                    autoDelete: false);

                _channel.QueueDeclare(
                    queue: "book.inventory.update",
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.QueueBind(
                    queue: "book.inventory.update",
                    exchange: _exchangeName,
                    routingKey: "inventory.update");

                _isConnected = true;
                _logger.LogInformation("RabbitMQ connection established successfully");
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                _isConnected = false;
                _logger.LogWarning(ex, "RabbitMQ server is unreachable. Messages will be logged but not sent to queue.");
                // Don't throw - allow the application to continue in degraded mode
            }
            catch (Exception ex)
            {
                _isConnected = false;
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                // Don't throw - allow the application to continue in degraded mode
            }
        }

        public Task PublishBookInventoryUpdateAsync(BookInventoryUpdateMessage message)
        {
            if (!_isConnected)
            {
                _logger.LogWarning($"Cannot send message - RabbitMQ connection not available. Message details: BookId={message.BookId}, Change={message.QuantityChange}");
                return Task.CompletedTask;
            }

            try
            {
                var messageJson = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: _exchangeName,
                    routingKey: "inventory.update",
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation($"Book inventory update message sent: BookId={message.BookId}, Change={message.QuantityChange}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send book inventory update message: BookId={message.BookId}");
                // Don't throw - don't let messaging failures break the main functionality
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _channel?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during RabbitMQ cleanup");
            }
        }
    }

    public class RabbitMQOptions
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public int Port { get; set; } = 5672;
    }
}