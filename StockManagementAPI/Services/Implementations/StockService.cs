using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using StockManagementApi.Models;
using StockManagementApi.Repositories.Interfaces;
using StockManagementApi.Services.Interfaces;
using StockManagementAPI.DTOs;
using StockManagementAPI.DTOs.Messages;
using StockManagementAPI.Services.Interfaces;
using System.Text.Json;

namespace StockManagementAPI.Services.Implementations
{
    public class StockService : IStockService
    {
        private readonly IStockTransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        private readonly ILogger<StockService> _logger;
        private readonly HttpClient _httpClient;

        public StockService(
            IStockTransactionRepository repository,
            IMapper mapper,
            IMessageService messageService,
            ILogger<StockService> logger,
            HttpClient httpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _messageService = messageService;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<StockTransactionDTO> GetTransactionByIdAsync(int id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            var dto = _mapper.Map<StockTransactionDTO>(transaction);

            // ✅ THÊM: Enrichment user names và book names
            await EnrichUserNamesAsync(dto);
            await EnrichBookNamesAsync(dto.Details);

            return dto;
        }

        // ✅ THÊM: Method để lấy user names
        private async Task EnrichUserNamesAsync(StockTransactionDTO transaction)
        {
            try
            {
                // Lấy Creator name
                if (transaction.CreatedBy > 0)
                {
                    var creatorName = await GetUserNameAsync(transaction.CreatedBy);
                    transaction.CreatorName = creatorName ?? $"User #{transaction.CreatedBy}";
                }

                // Lấy Approver name
                if (transaction.ApprovedBy.HasValue && transaction.ApprovedBy.Value > 0)
                {
                    var approverName = await GetUserNameAsync(transaction.ApprovedBy.Value);
                    transaction.ApproverName = approverName ?? $"User #{transaction.ApprovedBy}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enrich user names for transaction {TransactionId}", transaction.TransactionID);
            }
        }

        // ✅ THÊM: Method để lấy user name từ UserManagement API
        private async Task<string?> GetUserNameAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7073/api/users/admin/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(userJson);

                    // Thử parse nested response trước
                    if (document.RootElement.TryGetProperty("data", out var dataElement))
                    {
                        if (dataElement.TryGetProperty("fullname", out var fullNameElement))
                        {
                            return fullNameElement.GetString();
                        }
                    }
                    // Fallback: parse direct response
                    else if (document.RootElement.TryGetProperty("fullname", out var directFullNameElement))
                    {
                        return directFullNameElement.GetString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get user name for UserID: {UserId}", userId);
                return null;
            }
        }

        // ✅ CẬP NHẬT: Method để lấy book names và ISBN
        private async Task EnrichBookNamesAsync(ICollection<StockTransactionDetailDTO> details)
        {
            foreach (var detail in details)
            {
                try
                {
                    // Gọi BookManagement API để lấy book info
                    var response = await _httpClient.GetAsync($"https://localhost:7201/api/Book/{detail.BookID}/detailBook");
                    if (response.IsSuccessStatusCode)
                    {
                        var bookJson = await response.Content.ReadAsStringAsync();

                        // Parse JSON để lấy title và ISBN
                        using var document = JsonDocument.Parse(bookJson);

                        if (document.RootElement.TryGetProperty("title", out var titleElement))
                        {
                            detail.BookName = titleElement.GetString() ?? $"Book #{detail.BookID}";
                        }
                        else
                        {
                            detail.BookName = $"Book #{detail.BookID}";
                        }

                        if (document.RootElement.TryGetProperty("isbn", out var isbnElement))
                        {
                            detail.BookISBN = isbnElement.GetString();
                        }
                    }
                    else
                    {
                        detail.BookName = $"Book #{detail.BookID}";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get book name for BookID: {BookID}", detail.BookID);
                    detail.BookName = $"Book #{detail.BookID}";
                }
            }
        }

        public async Task<StockTransactionDTO> CreateTransactionAsync(CreateStockTransactionDTO createDto, int currentUserId)
        {
            // Chỉ chấp nhận StockIn, từ chối các loại khác
            if (createDto.TransactionType != "StockIn")
            {
                throw new ArgumentException("Chỉ chấp nhận giao dịch nhập kho (StockIn). Các giao dịch xuất kho được xử lý thông qua đơn đặt hàng.");
            }

            // ✅ Debug logging
            _logger.LogInformation($"🔥 Creating transaction with {createDto.Details?.Count ?? 0} details");

            var transaction = new StockTransaction
            {
                CreatedBy = currentUserId,
                TransactionType = "StockIn", // Đảm bảo chỉ là StockIn
                Status = "Pending",
                TransactionDate = DateTime.UtcNow,
                Note = createDto.Note,
                Details = _mapper.Map<List<StockTransactionDetail>>(createDto.Details)
            };

            // ✅ Debug logging mapped details
            _logger.LogInformation($"📦 Mapped {transaction.Details?.Count ?? 0} details to entity");

            if (transaction.Details != null)
            {
                foreach (var detail in transaction.Details)
                {
                    _logger.LogInformation($"📋 Detail: BookID={detail.BookID}, Quantity={detail.Quantity}, UnitPrice={detail.UnitPrice}");
                }
            }

            var result = await _repository.CreateAsync(transaction);

            // ✅ Debug logging sau khi save
            _logger.LogInformation($"✅ Transaction {result.TransactionID} created with {result.Details?.Count ?? 0} details");

            var dto = _mapper.Map<StockTransactionDTO>(result);

            // ✅ THÊM: Enrichment user names và book names
            await EnrichUserNamesAsync(dto);
            await EnrichBookNamesAsync(dto.Details);

            return dto;
        }

        public async Task<StockTransactionDTO> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusDTO updateDto, int currentUserId)
        {
            _logger.LogInformation($"🔥 Starting UpdateTransactionStatusAsync - TransactionID: {id}, Status: {updateDto.Status}, UserID: {currentUserId}");

            var transaction = await _repository.UpdateStatusAsync(id, updateDto.Status, currentUserId, updateDto.Note);

            if (transaction == null)
            {
                _logger.LogWarning($"❌ Transaction {id} not found or update failed");
                return null;
            }

            _logger.LogInformation($"✅ Transaction {id} status updated to {updateDto.Status}");

            // ✅ Nếu approved, gửi message để cập nhật số lượng sách
            if (updateDto.Status == "Approved" && transaction.TransactionType == "StockIn")
            {
                _logger.LogInformation($"🚀 Transaction {transaction.TransactionID} approved - Publishing inventory updates");
                _logger.LogInformation($"📦 Transaction details count: {transaction.Details?.Count ?? 0}");

                try
                {
                    await PublishInventoryUpdatesAsync(transaction);
                    _logger.LogInformation($"✅ Inventory updates published successfully for transaction {transaction.TransactionID}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"❌ Failed to publish inventory updates for transaction {transaction.TransactionID}");
                    throw; // Re-throw để transaction có thể rollback nếu cần
                }
            }
            else
            {
                _logger.LogInformation($"ℹ️ No inventory update needed - Status: {updateDto.Status}, Type: {transaction.TransactionType}");
            }

            var dto = _mapper.Map<StockTransactionDTO>(transaction);

            // ✅ THÊM: Enrichment user names và book names
            await EnrichUserNamesAsync(dto);
            await EnrichBookNamesAsync(dto.Details);

            return dto;
        }

        private async Task PublishInventoryUpdatesAsync(StockTransaction transaction)
        {
            _logger.LogInformation($"📨 PublishInventoryUpdatesAsync started for transaction {transaction.TransactionID}");

            if (transaction.Details == null || !transaction.Details.Any())
            {
                _logger.LogWarning($"⚠️ No details found for transaction {transaction.TransactionID}");
                return;
            }

            try
            {
                foreach (var detail in transaction.Details)
                {
                    // Luôn tăng số lượng cho StockIn
                    int quantityChange = detail.Quantity;

                    var message = new BookInventoryUpdateMessage
                    {
                        BookId = detail.BookID,
                        QuantityChange = quantityChange,
                        UnitPrice = detail.UnitPrice,
                        TransactionId = transaction.TransactionID,
                        TransactionType = "StockIn"
                    };

                    _logger.LogInformation($"📤 Publishing message for BookID {detail.BookID}: QuantityChange={quantityChange}, UnitPrice={detail.UnitPrice}");

                    await _messageService.PublishBookInventoryUpdateAsync(message);

                    _logger.LogInformation($"✅ Message published successfully for BookID {detail.BookID}");
                }

                _logger.LogInformation($"🎉 All inventory update messages published for transaction {transaction.TransactionID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Error publishing inventory updates for transaction {transaction.TransactionID}");
                throw; // Re-throw để transaction rollback nếu cần
            }
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public IQueryable<StockTransactionDTO> GetTransactionsQueryable()
        {
            // ✅ SỬA: Không sử dụng ProjectTo, thay vào đó return raw entity và sẽ map sau
            return _repository.GetQueryable()
                .Select(t => new StockTransactionDTO
                {
                    TransactionID = t.TransactionID,
                    CreatedBy = t.CreatedBy,
                    TransactionType = t.TransactionType,
                    Status = t.Status,
                    TransactionDate = t.TransactionDate,
                    ApprovedBy = t.ApprovedBy,
                    ApprovedAt = t.ApprovedAt,
                    Note = t.Note,
                    TotalItems = t.Details.Sum(d => d.Quantity),
                    TotalAmount = t.Details.Sum(d => d.Quantity * d.UnitPrice),
                    // ✅ Không map user names và book names ở đây để tránh lỗi translation
                    CreatorName = null,
                    ApproverName = null,
                    Details = t.Details.Select(d => new StockTransactionDetailDTO
                    {
                        DetailID = d.DetailID,
                        TransactionID = d.TransactionID,
                        BookID = d.BookID,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Note = d.Note,
                        BookName = null, // Sẽ được enriched sau
                        BookISBN = null
                    }).ToList()
                });
        }

        // ✅ THÊM: Method để đếm tổng số transactions với filter
        public async Task<int> GetTransactionCountAsync(string? searchTerm = null, string? transactionType = null,
            string? status = null, int? createdBy = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _repository.GetQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(t => t.Note != null && t.Note.ToLower().Contains(searchTerm.ToLower()));
                }

                if (!string.IsNullOrEmpty(transactionType))
                {
                    query = query.Where(t => t.TransactionType == transactionType);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(t => t.Status == status);
                }

                if (createdBy.HasValue)
                {
                    query = query.Where(t => t.CreatedBy == createdBy.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate <= toDate.Value);
                }

                var count = await query.CountAsync();
                _logger.LogInformation($"📊 Transaction count: {count} with filters applied");

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction count");
                return 0;
            }
        }
    }
}