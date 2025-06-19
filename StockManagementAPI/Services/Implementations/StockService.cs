using AutoMapper;
using AutoMapper.QueryableExtensions;
using StockManagementAPI.DTOs;
using StockManagementAPI.DTOs.Messages;
using StockManagementApi.Models;
using StockManagementApi.Repositories.Interfaces;
using StockManagementApi.Services.Interfaces;
using StockManagementAPI.Services.Interfaces;

namespace StockManagementAPI.Services.Implementations
{
    public class StockService : IStockService
    {
        private readonly IStockTransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        private readonly ILogger<StockService> _logger;

        public StockService(
            IStockTransactionRepository repository,
            IMapper mapper,
            IMessageService messageService,
            ILogger<StockService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<StockTransactionDTO> GetTransactionByIdAsync(int id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            return _mapper.Map<StockTransactionDTO>(transaction);
        }

        public async Task<StockTransactionDTO> CreateTransactionAsync(CreateStockTransactionDTO createDto, int currentUserId)
        {
            // Chỉ chấp nhận StockIn, từ chối các loại khác
            if (createDto.TransactionType != "StockIn")
            {
                throw new ArgumentException("Chỉ chấp nhận giao dịch nhập kho (StockIn). Các giao dịch xuất kho được xử lý thông qua đơn đặt hàng.");
            }

            var transaction = new StockTransaction
            {
                CreatedBy = currentUserId,
                TransactionType = "StockIn", // Đảm bảo chỉ là StockIn
                Status = "Pending",
                TransactionDate = DateTime.UtcNow,
                Note = createDto.Note,
                Details = _mapper.Map<List<StockTransactionDetail>>(createDto.Details)
            };

            var result = await _repository.CreateAsync(transaction);
            return _mapper.Map<StockTransactionDTO>(result);
        }

        public async Task<StockTransactionDTO> UpdateTransactionStatusAsync(int id, UpdateTransactionStatusDTO updateDto, int currentUserId)
        {
            var transaction = await _repository.UpdateStatusAsync(id, updateDto.Status, currentUserId, updateDto.Note);

            // Nếu approved, gửi message để cập nhật số lượng sách (chỉ tăng)
            if (transaction != null && updateDto.Status == "Approved" && transaction.TransactionType == "StockIn")
            {
                await PublishInventoryUpdatesAsync(transaction);
            }

            return _mapper.Map<StockTransactionDTO>(transaction);
        }

        private async Task PublishInventoryUpdatesAsync(StockTransaction transaction)
        {
            try
            {
                foreach (var detail in transaction.Details)
                {
                    // Luôn tăng số lượng (không còn cần switch-case)
                    int quantityChange = detail.Quantity;

                    var message = new BookInventoryUpdateMessage
                    {
                        BookId = detail.BookID,
                        QuantityChange = quantityChange,
                        UnitPrice = detail.UnitPrice,
                        TransactionId = transaction.TransactionID,
                        TransactionType = "StockIn"
                    };

                    await _messageService.PublishBookInventoryUpdateAsync(message);
                    _logger.LogInformation($"Inventory update message published: BookId={detail.BookID}, Change={quantityChange}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing inventory updates for transaction {transaction.TransactionID}");
            }
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public IQueryable<StockTransactionDTO> GetTransactionsQueryable()
        {
            return _repository.GetQueryable()
                .ProjectTo<StockTransactionDTO>(_mapper.ConfigurationProvider);
        }
    }
}