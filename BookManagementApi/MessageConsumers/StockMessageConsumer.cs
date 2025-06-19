using BookManagementApi.DTOs.Messages;
using BookManagementApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BookManagementApi.MessageConsumers
{
    public class StockMessageConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StockMessageConsumer> _logger;
        private const string QueueName = "book.inventory.update";

        public StockMessageConsumer(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<StockMessageConsumer> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" }; // Lấy từ cấu hình
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                    
                _logger.LogInformation("RabbitMQ consumer initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ consumer");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not available");
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var inventoryUpdate = JsonSerializer.Deserialize<BookInventoryUpdateMessage>(message);

                    _logger.LogInformation($"Nhận thông báo cập nhật tồn kho: BookId={inventoryUpdate.BookId}, Change={inventoryUpdate.QuantityChange}");

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
                            _logger.LogInformation($"Đã cập nhật tồn kho thành công: BookId={inventoryUpdate.BookId}");
                        }
                        else
                        {
                            // Nếu không tìm thấy book hoặc lỗi, requeue message
                            _channel.BasicNack(ea.DeliveryTag, false, true);
                            _logger.LogWarning($"Không thể cập nhật tồn kho: BookId={inventoryUpdate.BookId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi xử lý message cập nhật tồn kho");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}