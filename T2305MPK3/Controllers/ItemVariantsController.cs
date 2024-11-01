using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemVariantsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ItemVariantsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new ItemVariant
        [HttpPost]
        public async Task<IActionResult> CreateItemVariant([FromBody] ItemVariants itemVariant)
        {
            // Check if MenuItem and Size exist
            if (!await _dbContext.MenuItems.AnyAsync(mi => mi.MenuItemNo == itemVariant.MenuItemNo) ||
                !await _dbContext.Sizes.AnyAsync(s => s.SizeId == itemVariant.SizeId))
            {
                return BadRequest("Invalid MenuItem or Size.");
            }

            _dbContext.ItemVariants.Add(itemVariant);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemVariantById), new { id = itemVariant.VariantId }, itemVariant);
        }

        // Get a specific ItemVariant by VariantId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemVariantById(long id)
        {
            var itemVariant = await _dbContext.ItemVariants
                .Include(iv => iv.MenuItem)
                .Include(iv => iv.Size)
                .FirstOrDefaultAsync(iv => iv.VariantId == id);

            if (itemVariant == null)
            {
                return NotFound();
            }

            return Ok(itemVariant);
        }

        // Get all ItemVariants by MenuItemNo
        [HttpGet("by-menuitem/{menuItemNo}")]
        public async Task<IActionResult> GetItemVariantsByMenuItemNo(int menuItemNo)
        {
            var itemVariants = await _dbContext.ItemVariants
                .Where(iv => iv.MenuItemNo == menuItemNo)
                .Include(iv => iv.MenuItem)
                .Include(iv => iv.Size)
                .ToListAsync();

            return Ok(itemVariants);
        }

        // Update an ItemVariant
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemVariant(long id, [FromBody] ItemVariants updatedItemVariant)
        {
            var existingItemVariant = await _dbContext.ItemVariants.FindAsync(id);
            if (existingItemVariant == null)
            {
                return NotFound();
            }

            // Update properties
            existingItemVariant.MenuItemNo = updatedItemVariant.MenuItemNo;
            existingItemVariant.SizeId = updatedItemVariant.SizeId;
            existingItemVariant.Price = updatedItemVariant.Price;

            _dbContext.ItemVariants.Update(existingItemVariant);
            await _dbContext.SaveChangesAsync();

            return Ok(existingItemVariant);
        }

        // Delete an ItemVariant
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemVariant(long id)
        {
            var itemVariant = await _dbContext.ItemVariants.FindAsync(id);
            if (itemVariant == null)
            {
                return NotFound();
            }

            _dbContext.ItemVariants.Remove(itemVariant);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
