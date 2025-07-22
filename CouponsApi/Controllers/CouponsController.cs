using CouponsApi.DTOs;
using CouponsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CouponsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponsController(ICouponService service)
        {
            _service = service;
        }

        // OData endpoint: /odata/Coupons
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CouponReadDto>>> GetCoupons()
        {
            var coupons = await _service.GetAllAsync();
            return Ok(coupons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponReadDto>> GetCoupon(int id)
        {
            try {
                var coupon = await _service.GetByIdAsync(id);
                return Ok(coupon);
            }
            catch (KeyNotFoundException) {
                return NotFound("Coupon not found or inactive");
            }
        }

        [HttpGet("bycode/{code}")]
        public async Task<ActionResult<CouponReadDto>> GetCouponByCode(string code)
        {
            try {
                var coupon = await _service.GetByCodeAsync(code);
                return Ok(coupon);
            }
            catch (KeyNotFoundException) {
                return NotFound("Coupon not found or inactive");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CouponReadDto>> PostCoupon(CouponDTO couponDto)
        {
            try {
                var coupon = await _service.CreateAsync(couponDto);
                return CreatedAtAction(nameof(GetCoupon), new { id = coupon.CouponID }, coupon);
            }
            catch (FluentValidation.ValidationException ex) {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoupon(int id, CouponUpdateDto couponDto)
        {
            if (id != couponDto.CouponID)
                return BadRequest("CouponID mismatch");
            try {
                await _service.UpdateAsync(id, couponDto);
                return NoContent();
            }
            catch (FluentValidation.ValidationException ex) {
                return BadRequest(ex.Errors);
            }
            catch (KeyNotFoundException) {
                return NotFound("Coupon not found or inactive");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (FluentValidation.ValidationException ex) {
                return BadRequest(ex.Errors);
            }
            catch (KeyNotFoundException) {
                return NotFound("Coupon not found or already inactive");
            }
        }
    }
}