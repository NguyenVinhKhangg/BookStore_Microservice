using AutoMapper;
using StockManagementApi.Models;
using StockManagementAPI.DTOs;

namespace StockManagementApi.Profiles
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            // Transaction mapping
            CreateMap<StockTransaction, StockTransactionDTO>();
            
            // Transaction detail mapping
            CreateMap<StockTransactionDetail, StockTransactionDetailDTO>();
            
            // Create DTOs to entities
            CreateMap<CreateStockTransactionDetailDTO, StockTransactionDetail>();
        }
    }
}