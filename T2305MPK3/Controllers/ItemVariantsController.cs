using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.DTOs;
using T2305MPK3.Models;
using System.Threading.Tasks;
using System.Linq;

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
        public async Task<IActionResult> CreateItemVariant([FromBody] ItemVariantDTO itemVariantDto)
        {
            var sizeExists = await _dbContext.Sizes.AnyAsync(s => s.SizeId == itemVariantDto.SizeId);
            var menuItemExists = await _dbContext.MenuItems.AnyAsync(mi => mi.MenuItemNo == itemVariantDto.MenuItemNo);

            if (!sizeExists)
            {
                return BadRequest($"Size with ID {itemVariantDto.SizeId} does not exist.");
            }

            if (!menuItemExists)
            {
                return BadRequest($"MenuItem with ID {itemVariantDto.MenuItemNo} does not exist.");
            }

            var itemVariant = new ItemVariants
            {
                Price = itemVariantDto.Price,
                SizeId = itemVariantDto.SizeId,
                MenuItemNo = itemVariantDto.MenuItemNo
            };

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

            // Map entity to DTO to avoid circular references
            var itemVariantDto = new ItemVariantDTO
            {
                VariantId = itemVariant.VariantId,
                Price = itemVariant.Price,
                SizeId = itemVariant.SizeId,
                MenuItemNo = itemVariant.MenuItemNo,
                MenuItemName = itemVariant.MenuItem?.ItemName,
                SizeNumber = itemVariant.Size?.SizeNumber
            };

            return Ok(itemVariantDto);
        }

        // Get all ItemVariants
        [HttpGet]
        public async Task<IActionResult> GetAllItemVariants()
        {
            var itemVariants = await _dbContext.ItemVariants
                .Include(iv => iv.MenuItem)
                .Include(iv => iv.Size)
                .ToListAsync();

            // Map to DTOs
            var itemVariantDtos = itemVariants.Select(iv => new ItemVariantDTO
            {
                VariantId = iv.VariantId,
                Price = iv.Price,
                SizeId = iv.SizeId,
                MenuItemNo = iv.MenuItemNo,
                MenuItemName = iv.MenuItem?.ItemName,
                SizeNumber = iv.Size?.SizeNumber
            }).ToList();

            return Ok(itemVariantDtos);
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

            var itemVariantDtos = itemVariants.Select(iv => new ItemVariantDTO
            {
                VariantId = iv.VariantId,
                Price = iv.Price,
                SizeId = iv.SizeId,
                MenuItemNo = iv.MenuItemNo,
                MenuItemName = iv.MenuItem?.ItemName,
                SizeNumber = iv.Size?.SizeNumber
            }).ToList();

            return Ok(itemVariantDtos);
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
