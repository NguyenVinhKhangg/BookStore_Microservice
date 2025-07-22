using AutoMapper;
using OrdersManagementApi.DTOs;
using OrdersManagementApi.Models;
using OrdersManagementApi.Repository;

namespace OrdersManagementApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly HttpClient _httpClient;

        public OrderService(
            IOrderRepository repository,
            IMapper mapper,
            ILogger<OrderService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory.CreateClient();
        }

        // ✅ Basic CRUD Operations
        public async Task<IEnumerable<OrderReadDto>> GetAllAsync()
        {
            try
            {
                var orders = await _repository.GetAllAsync();
                var orderDtos = _mapper.Map<IEnumerable<OrderReadDto>>(orders);

                // Enrich with user information
                foreach (var orderDto in orderDtos)
                {
                    await EnrichUserInformationAsync(orderDto);
                }

                return orderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public async Task<OrderReadDto?> GetByIdAsync(int id)
        {
            try
            {
                var order = await _repository.GetByIdAsync(id);
                if (order == null) return null;

                var orderDto = _mapper.Map<OrderReadDto>(order);
                await EnrichUserInformationAsync(orderDto);

                return orderDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {id}");
                throw;
            }
        }

        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            try
            {
                var order = _mapper.Map<Order>(orderDto);
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";

                // Calculate total amount
                order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);

                // Enrich book information
                await EnrichBookInformationAsync(order.OrderItems);

                var savedOrder = await _repository.AddAsync(order);

                _logger.LogInformation($"✅ Order {savedOrder.OrderID} created for user {orderDto.UserID}");

                return _mapper.Map<OrderReadDto>(savedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating order for user {orderDto.UserID}");
                throw;
            }
        }

        //public async Task<bool> UpdateOrderAsync(int id, OrderUpdateDto orderDto)
        //{
        //    try
        //    {
        //        var order = await _repository.GetByIdAsync(id);
        //        if (order == null) return false;

        //        _mapper.Map(orderDto, order);
        //        await _repository.UpdateAsync(order);

        //        _logger.LogInformation($"✅ Order {id} updated");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error updating order {id}");
        //        throw;
        //    }
        //}

        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                _logger.LogInformation($"✅ Order {id} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return false;
            }
        }

        // ✅ Checkout từ cart
        public async Task<OrderReadDto> CheckoutFromCartAsync(CheckoutDto checkoutDto)
        {
            try
            {
                // Calculate total
                var totalAmount = checkoutDto.Items.Sum(item => item.Quantity * item.UnitPrice);

                // Create order
                var order = new Order
                {
                    UserID = checkoutDto.UserID,
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending", // ✅ Pending cho admin approve
                    Notes = checkoutDto.Notes,
                    OrderItems = checkoutDto.Items.Select(item => new OrderItem
                    {
                        BookID = item.BookID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        BookTitle = item.BookTitle,
                        BookISBN = item.BookISBN
                    }).ToList()
                };

                // Enrich book information
                await EnrichBookInformationAsync(order.OrderItems);

                // Save order
                var savedOrder = await _repository.AddAsync(order);

                _logger.LogInformation($"✅ Order {savedOrder.OrderID} created from checkout for user {checkoutDto.UserID} with {order.OrderItems.Count} items");

                var result = _mapper.Map<OrderReadDto>(savedOrder);
                await EnrichUserInformationAsync(result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error creating order from checkout for user {checkoutDto.UserID}");
                throw;
            }
        }

        // ✅ Admin update order status
        public async Task<OrderReadDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto statusDto, int adminUserId)
        {
            try
            {
                var order = await _repository.GetByIdAsync(orderId);
                if (order == null)
                    throw new KeyNotFoundException($"Order {orderId} not found");

                var oldStatus = order.Status;
                order.Status = statusDto.Status;
                order.AdminNotes = statusDto.AdminNotes;

                if (statusDto.Status == "Confirmed" && oldStatus != "Confirmed")
                {
                    order.ConfirmedAt = DateTime.UtcNow;
                    order.ConfirmedBy = adminUserId;

                    // ✅ Trigger stock reduction
                    await ProcessStockReductionAsync(order);
                }

                await _repository.UpdateAsync(order);

                _logger.LogInformation($"✅ Order {orderId} status updated from {oldStatus} to {statusDto.Status} by admin {adminUserId}");

                var result = _mapper.Map<OrderReadDto>(order);
                await EnrichUserInformationAsync(result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {orderId} status");
                throw;
            }
        }

        // ✅ Get orders cho admin với filters và pagination
        public async Task<(IEnumerable<OrderSummaryDto> Orders, int TotalCount)> GetOrdersForAdminAsync(
            string? searchTerm = null,
            string? statusFilter = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var orders = await _repository.GetAllAsync();

                var filteredOrders = orders.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filteredOrders = filteredOrders.Where(o =>
                        o.OrderID.ToString().Contains(searchTerm) ||
                        o.UserID.ToString().Contains(searchTerm) ||
                        (o.Notes != null && o.Notes.Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    filteredOrders = filteredOrders.Where(o => o.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
                }

                if (fromDate.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.OrderDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.OrderDate <= toDate.Value);
                }

                var totalCount = filteredOrders.Count();
                var pagedOrders = filteredOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var orderSummaries = pagedOrders.Select(o => new OrderSummaryDto
                {
                    OrderID = o.OrderID,
                    UserID = o.UserID,
                    UserName = $"User #{o.UserID}", // TODO: Get from UserManagement API
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalItems = o.OrderItems?.Sum(item => item.Quantity) ?? 0
                });

                // Enrich user information
                foreach (var summary in orderSummaries)
                {
                    summary.UserName = await GetUserNameAsync(summary.UserID) ?? $"User #{summary.UserID}";
                }

                return (orderSummaries, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for admin");
                throw;
            }
        }

        // ✅ Get orders by user
        public async Task<(IEnumerable<OrderReadDto> Orders, int TotalCount)> GetOrdersByUserAsync(
            int userId,
            string? statusFilter = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var orders = await _repository.GetAllAsync();

                var userOrders = orders.Where(o => o.UserID == userId);

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    userOrders = userOrders.Where(o => o.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
                }

                var totalCount = userOrders.Count();
                var pagedOrders = userOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var orderDtos = _mapper.Map<IEnumerable<OrderReadDto>>(pagedOrders);

                return (orderDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {userId}");
                throw;
            }
        }

        // ✅ Legacy methods (compatibility)
        public async Task<(IEnumerable<OrderReadDto> Orders, int TotalCount)> GetOrdersAsync(
            string searchTerm,
            string statusFilter,
            int page,
            int pageSize)
        {
            var (orders, totalCount) = await GetOrdersForAdminAsync(searchTerm, statusFilter, null, null, page, pageSize);
            var orderReadDtos = orders.Select(summary => new OrderReadDto
            {
                OrderID = summary.OrderID,
                UserID = summary.UserID,
                TotalAmount = summary.TotalAmount,
                OrderDate = summary.OrderDate,
                Status = summary.Status,
                OrderItems = new List<OrderItemReadDto>()
            });

            return (orderReadDtos, totalCount);
        }

        public async Task ConfirmAsync(int id)
        {
            await UpdateOrderStatusAsync(id, new UpdateOrderStatusDto { Status = "Confirmed" }, 1);
        }

        // ✅ Statistics methods
        public async Task<int> GetOrderCountAsync(string? statusFilter = null)
        {
            try
            {
                var orders = await _repository.GetAllAsync();

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    return orders.Count(o => o.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
                }

                return orders.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order count");
                return 0;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var orders = await _repository.GetAllAsync();

                var confirmedOrders = orders.Where(o => o.Status == "Confirmed" || o.Status == "Delivered");

                if (fromDate.HasValue)
                {
                    confirmedOrders = confirmedOrders.Where(o => o.OrderDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    confirmedOrders = confirmedOrders.Where(o => o.OrderDate <= toDate.Value);
                }

                return confirmedOrders.Sum(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return 0;
            }
        }

        public async Task<IDictionary<string, int>> GetOrderStatusStatsAsync()
        {
            try
            {
                var orders = await _repository.GetAllAsync();

                return orders
                    .GroupBy(o => o.Status)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order status stats");
                return new Dictionary<string, int>();
            }
        }

        // ✅ Helper methods
        private async Task EnrichBookInformationAsync(ICollection<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                try
                {
                    // Call BookManagement API để get book details
                    var response = await _httpClient.GetAsync($"https://localhost:7201/api/Book/{item.BookID}/detailBook");
                    if (response.IsSuccessStatusCode)
                    {
                        var bookJson = await response.Content.ReadAsStringAsync();
                        using var document = System.Text.Json.JsonDocument.Parse(bookJson);

                        if (document.RootElement.TryGetProperty("title", out var titleElement))
                        {
                            item.BookTitle = titleElement.GetString();
                        }

                        if (document.RootElement.TryGetProperty("isbn", out var isbnElement))
                        {
                            item.BookISBN = isbnElement.GetString();
                        }

                        if (document.RootElement.TryGetProperty("imageUrl", out var imageElement))
                        {
                            item.BookImageUrl = imageElement.GetString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to enrich book information for BookID {item.BookID}");
                    item.BookTitle = $"Book #{item.BookID}";
                }
            }
        }

        private async Task EnrichUserInformationAsync(OrderReadDto orderDto)
        {
            try
            {
                var userName = await GetUserNameAsync(orderDto.UserID);
                // You can add UserName property to OrderReadDto if needed
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to enrich user information for UserID {orderDto.UserID}");
            }
        }

        private async Task<string?> GetUserNameAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7073/api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    using var document = System.Text.Json.JsonDocument.Parse(userJson);

                    if (document.RootElement.TryGetProperty("fullname", out var fullNameElement))
                    {
                        return fullNameElement.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to get user name for UserID {userId}");
            }

            return null;
        }

        private async Task ProcessStockReductionAsync(Order order)
        {
            try
            {
                // TODO: Send message to StockManagement để reduce stock khi order confirmed
                // This can be implemented với RabbitMQ hoặc direct API call

                foreach (var item in order.OrderItems)
                {
                    _logger.LogInformation($"📦 Stock reduction needed: BookID {item.BookID}, Quantity {item.Quantity}");

                    // Example: Call StockManagement API
                    // await _httpClient.PostAsync($"https://localhost:7189/api/stock/reduce", ...);
                }

                _logger.LogInformation($"📦 Stock reduction triggered for order {order.OrderID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process stock reduction for order {order.OrderID}");
                // Don't throw - stock reduction failure shouldn't block order confirmation
            }
        }
    }
}