using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SizesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public SizesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new Size
        [HttpPost]
        public async Task<IActionResult> CreateSize([FromBody] Sizes size)
        {
            _dbContext.Sizes.Add(size);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSizeById), new { id = size.SizeId }, size);
        }

        // Get a Size by SizeId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(long id)
        {
            var size = await _dbContext.Sizes.FindAsync(id);
            if (size == null)
            {
                return NotFound();
            }

            return Ok(size);
        }

        // Get all Sizes
        [HttpGet]
        public async Task<IActionResult> GetAllSizes()
        {
            var sizes = await _dbContext.Sizes.ToListAsync();
            return Ok(sizes);
        }

        // Update a Size
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSize(long id, [FromBody] Sizes updatedSize)
        {
            var existingSize = await _dbContext.Sizes.FindAsync(id);
            if (existingSize == null)
            {
                return NotFound();
            }

            existingSize.SizeNumber = updatedSize.SizeNumber;

            _dbContext.Sizes.Update(existingSize);
            await _dbContext.SaveChangesAsync();

            return Ok(existingSize);
        }

        // Delete a Size
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(long id)
        {
            var size = await _dbContext.Sizes.FindAsync(id);
            if (size == null)
            {
                return NotFound();
            }

            _dbContext.Sizes.Remove(size);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
