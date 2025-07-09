using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using StockManagementAPI.DTOs;
using StockManagementApi.Services.Interfaces;
using System.Security.Claims;

namespace StockManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        
        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }
        
        [Authorize(Roles = "Admin,Staff")]
        [EnableQuery]
        [HttpGet("/odata/transactions")]
        public IQueryable<StockTransactionDTO> GetTransactionsOData()
        {
            return _stockService.GetTransactionsQueryable();
        }
        
        //[Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var transaction = await _stockService.GetTransactionByIdAsync(id);
                
                if (transaction == null)
                    return NotFound(new { success = false, message = $"Transaction with ID {id} not found" });
                    
                return Ok(new { success = true, data = transaction });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateStockTransactionDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                    
                //var currentUserId = GetCurrentUserId();
                var transaction = await _stockService.CreateTransactionAsync(createDto, 3);
                
                return Ok(new { 
                    success = true, 
                    message = "Transaction created successfully", 
                    data = transaction 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTransactionStatus(int id, [FromBody] UpdateTransactionStatusDTO updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                    
                var currentUserId = GetCurrentUserId();
                var transaction = await _stockService.UpdateTransactionStatusAsync(id, updateDto, currentUserId);
                
                if (transaction == null)
                    return NotFound(new { success = false, message = $"Transaction with ID {id} not found" });
                    
                return Ok(new { 
                    success = true, 
                    message = $"Transaction status updated to {updateDto.Status}", 
                    data = transaction 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var result = await _stockService.DeleteTransactionAsync(id);
                
                if (!result)
                    return NotFound(new { success = false, message = $"Transaction with ID {id} not found" });
                    
                return Ok(new { 
                    success = true, 
                    message = "Transaction deleted successfully" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        #region Helper Methods
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid token or user ID not found");
            }
            return userId;
        }
        #endregion
    }
}