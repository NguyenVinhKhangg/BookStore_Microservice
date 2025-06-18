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
            var transaction = new StockTransaction
            {
                CreatedBy = currentUserId,
                TransactionType = createDto.TransactionType,
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
            
            // Nếu approved, gửi message để cập nhật số lượng sách
            if (transaction != null && updateDto.Status == "Approved")
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
                    // Xác định QuantityChange dựa trên loại transaction
                    int quantityChange = transaction.TransactionType switch
                    {
                        "StockIn" => detail.Quantity,
                        "StockOut" => -detail.Quantity,
                        "Adjustment" => detail.Quantity, // Xem xét lại logic cho Adjustment
                        _ => 0
                    };
                    
                    // Bỏ qua nếu không có thay đổi số lượng
                    if (quantityChange == 0)
                        continue;
                    
                    // Tạo và gửi message
                    var message = new BookInventoryUpdateMessage
                    {
                        BookId = detail.BookID,
                        QuantityChange = quantityChange,
                        UnitPrice = detail.UnitPrice,
                        TransactionId = transaction.TransactionID,
                        TransactionType = transaction.TransactionType
                    };
                    
                    await _messageService.PublishBookInventoryUpdateAsync(message);
                    _logger.LogInformation($"Inventory update message published: BookId={detail.BookID}, Change={quantityChange}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing inventory updates for transaction {transaction.TransactionID}");
                // Có thể xem xét throw exception hoặc xử lý retry ở đây
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