using AutoMapper;
using StockManagementAPI.DTOs;
using StockManagementApi.Models;

namespace StockManagementApi.Profiles
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            // ✅ SỬA: Map StockTransaction -> StockTransactionDTO đơn giản hơn cho ProjectTo
            CreateMap<StockTransaction, StockTransactionDTO>()
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Details.Sum(d => d.Quantity)))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Details.Sum(d => d.Quantity * d.UnitPrice)))
                // ✅ SỬA: Map null thay vì Ignore để tránh lỗi translation
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => (string)null))
                .ForMember(dest => dest.ApproverName, opt => opt.MapFrom(src => (string)null));

            // ✅ Map CreateStockTransactionDTO -> StockTransaction
            CreateMap<CreateStockTransactionDTO, StockTransaction>()
                .ForMember(dest => dest.TransactionID, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedAt, opt => opt.Ignore());

            // ✅ SỬA: Map StockTransactionDetail -> StockTransactionDetailDTO đơn giản
            CreateMap<StockTransactionDetail, StockTransactionDetailDTO>()
                .ForMember(dest => dest.BookName, opt => opt.MapFrom(src => (string)null)) // Map null thay vì Ignore
                .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => (string)null)); // Map null thay vì Ignore

            // ✅ Map CreateStockTransactionDetailDTO -> StockTransactionDetail
            CreateMap<CreateStockTransactionDetailDTO, StockTransactionDetail>()
               .ForMember(dest => dest.DetailID, opt => opt.Ignore())
               .ForMember(dest => dest.TransactionID, opt => opt.Ignore())
               .ForMember(dest => dest.BookID, opt => opt.MapFrom(src => src.BookID))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
               .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));
        }
    }
}