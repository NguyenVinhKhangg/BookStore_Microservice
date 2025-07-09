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

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        /*  [HttpGet]
          public async Task<IActionResult> GetAll()
          {
              var result = await _service.GetAllAsync();
              return Ok(result);
          }*/
        //[Authorize(Roles = "Admin,User,Staff")]
        [EnableQuery]
        [HttpGet]
        public IQueryable<CategoryDTO> GetAll()
        {
            return _service.GetCategoriesQueryable();
        }

        //[Authorize(Roles = "Admin,User,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        //[Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.CategoryID }, result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var success = await _service.ActivateAsync(id);
            if (!success) return NotFound();
            return Ok(new { success = true, message = "Category activated successfully" });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _service.DeactivateAsync(id);
            if (!success) return NotFound();
            return Ok(new { success = true, message = "Category deactivated successfully" });
        }
    }
}