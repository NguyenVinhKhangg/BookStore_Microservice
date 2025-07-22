using CategoryManagementApi.DTOs;
using CategoryManagementApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CategoryManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService service, ILogger<CategoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ✅ Public OData endpoint for browsing categories
        [AllowAnonymous]
        [EnableQuery]
        [HttpGet("/odata/categories")]
        public IQueryable<CategoryDTO> GetAll()
        {
            return _service.GetCategoriesQueryable();
        }

        // ✅ Public endpoint for total count
        [AllowAnonymous]
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalCategories()
        {
            try
            {
                var total = await _service.GetTotalAsync();
                _logger.LogInformation($"Total categories count: {total}");

                return Ok(new
                {
                    success = true,
                    data = new { totalCategories = total }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total categories count");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ✅ Public endpoint for viewing single category - FIXED RESPONSE FORMAT
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by id: {Id}", id);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ✅ Admin/Staff only - Create category - FIXED RESPONSE FORMAT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = string.Join("; ", errors)
                    });
                }

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.CategoryID }, new
                {
                    success = true,
                    message = "Category created successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ✅ Admin/Staff only - Update category - FIXED RESPONSE FORMAT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = string.Join("; ", errors)
                    });
                }

                var result = await _service.UpdateAsync(id, dto);
                return Ok(new
                {
                    success = true,
                    message = "Category updated successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {Id}", id);

                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Category not found"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ✅ Admin/Staff only - Delete category - FIXED RESPONSE FORMAT
        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Category deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {Id}", id);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ✅ Admin/Staff only - Activate category - ALREADY CORRECT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var success = await _service.ActivateAsync(id);
                if (!success)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Category activated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating category: {Id}", id);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ✅ Admin/Staff only - Deactivate category - ALREADY CORRECT
        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                var success = await _service.DeactivateAsync(id);
                if (!success)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Category not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Category deactivated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating category: {Id}", id);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}