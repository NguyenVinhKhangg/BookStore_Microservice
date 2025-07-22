using StockManagementAPI.DTOs.Messages;

namespace StockManagementAPI.Services.Interfaces
{
    public interface IMessageService
    {
        Task PublishBookInventoryUpdateAsync(BookInventoryUpdateMessage message);
    }
}