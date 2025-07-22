using BookManagementApi.DTOs.Messages;
using BookManagementApi.Options;
using BookManagementApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BookManagementApi.MessageConsumers
{
    public class StockMessageConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StockMessageConsumer> _logger;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private const string QueueName = "book.inventory.update";
        private bool _isConnected = false;

        public StockMessageConsumer(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<StockMessageConsumer> logger,
            IOptions<RabbitMQOptions> rabbitMQOptions)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _rabbitMQOptions = rabbitMQOptions.Value;

            TryConnect();
        }

        private void TryConnect()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMQOptions.HostName,
                    Port = _rabbitMQOptions.Port,
                    UserName = _rabbitMQOptions.UserName,
                    Password = _rabbitMQOptions.Password,
                    VirtualHost = _rabbitMQOptions.VirtualHost,
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
                };

                _logger.LogInformation($"🔌 Attempting to connect to RabbitMQ at {_rabbitMQOptions.HostName}:{_rabbitMQOptions.Port}");

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _isConnected = true;
                _logger.LogInformation("✅ RabbitMQ consumer initialized successfully");
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                _isConnected = false;
                _logger.LogWarning(ex, "⚠️ RabbitMQ server is unreachable. Consumer will run in degraded mode.");
            }
            catch (Exception ex)
            {
                _isConnected = false;
                _logger.LogError(ex, "❌ Failed to initialize RabbitMQ consumer");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_isConnected || _channel == null)
            {
                _logger.LogWarning("⚠️ RabbitMQ channel is not available. Consumer will not process messages.");
                return Task.CompletedTask;
            }

            _logger.LogInformation("🚀 Starting RabbitMQ message consumer...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"📥 Received message: {message}");

                try
                {
                    var inventoryUpdate = JsonSerializer.Deserialize<BookInventoryUpdateMessage>(message);

                    _logger.LogInformation($"🔥 Processing inventory update: BookId={inventoryUpdate.BookId}, QuantityChange={inventoryUpdate.QuantityChange}, TransactionId={inventoryUpdate.TransactionId}");

                    // Tạo scope mới để resolve scoped services
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

                        // Cập nhật số lượng sách
                        bool success = await bookService.UpdateBookStockAsync(
                            inventoryUpdate.BookId,
                            inventoryUpdate.QuantityChange);

                        if (success)
                        {
                            _channel.BasicAck(ea.DeliveryTag, false);
                            _logger.LogInformation($"✅ Successfully updated stock for BookId={inventoryUpdate.BookId}, Change={inventoryUpdate.QuantityChange}");
                        }
                        else
                        {
                            // Nếu không tìm thấy book hoặc lỗi, requeue message
                            _channel.BasicNack(ea.DeliveryTag, false, true);
                            _logger.LogWarning($"❌ Failed to update stock for BookId={inventoryUpdate.BookId}, message requeued");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"💥 Error processing inventory update message: {message}");
                    if (_channel?.IsOpen == true)
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation($"👂 Consumer is listening on queue: {QueueName}");

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            try
            {
                _logger.LogInformation("🔌 Disposing RabbitMQ connections...");
                _channel?.Close();
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error disposing RabbitMQ connections");
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}