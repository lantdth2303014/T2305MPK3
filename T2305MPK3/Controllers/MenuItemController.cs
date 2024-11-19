using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.DTOs;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public MenuItemController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new MenuItem
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemDTO menuItemDto)
        {
            var menuItem = new MenuItem
            {
                ItemName = menuItemDto.ItemName,
                Price = menuItemDto.Price,
                CategoryId = menuItemDto.CategoryId,
                Type = menuItemDto.Type,
                Description = menuItemDto.Description,
                Ingredient = menuItemDto.Ingredient,
                ImageURL = menuItemDto.ImageURL
            };

            _dbContext.MenuItems.Add(menuItem);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.MenuItemNo }, menuItem);
        }

        // Get a MenuItem by MenuItemNo (ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var menuItem = await _dbContext.MenuItems.Include(mi => mi.Category).FirstOrDefaultAsync(mi => mi.MenuItemNo == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return Ok(menuItem);
        }

        // Get all MenuItems by CategoryId
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetMenuItemsByCategoryId(int categoryId)
        {
            // Fetch MenuItems with related ItemVariants and Category
            var menuItems = await _dbContext.MenuItems
                .Where(mi => mi.CategoryId == categoryId)
                .Include(mi => mi.ItemVariants) // Include ItemVariants
                .Include(mi => mi.Category)    // Include Category
                .ToListAsync();

            // Map to DTOs to avoid circular references
            var menuItemDtos = menuItems.Select(mi => new MenuItemDTO
            {
                MenuItemNo = mi.MenuItemNo,
                ItemName = mi.ItemName,
                Price = mi.Price,
                CategoryId = mi.CategoryId,
                Type = mi.Type,
                Description = mi.Description,
                Ingredient = mi.Ingredient,
                ImageURL = mi.ImageURL,
                Category = new CategoryDTO
                {
                    CategoryId = mi.Category?.CategoryId ?? 0,
                    CategoryName = mi.Category?.CategoryName
                },
                ItemVariants = mi.ItemVariants.Select(iv => new ItemVariantDTO
                {
                    VariantId = iv.VariantId,
                    Price = iv.Price,
                    SizeId = iv.SizeId,
                    MenuItemNo = iv.MenuItemNo
                }).ToList()
            }).ToList();

            return Ok(menuItemDtos);
        }

        // Get all MenuItems with their Variants
        [HttpGet("with-variants")]
        public async Task<IActionResult> GetMenuItemsWithVariants()
        {
            var menuItems = await _dbContext.MenuItems
                .Include(mi => mi.ItemVariants)
                .ToListAsync();

            // Map entities to DTOs
            var menuItemDtos = menuItems.Select(mi => new MenuItemDTO
            {
                MenuItemNo = mi.MenuItemNo,
                ItemName = mi.ItemName,
                Price = mi.Price,
                CategoryId = mi.CategoryId,
                Type = mi.Type,
                Description = mi.Description,
                Ingredient = mi.Ingredient,
                ImageURL = mi.ImageURL,
                ItemVariants = mi.ItemVariants.Select(iv => new ItemVariantDTO
                {
                    VariantId = iv.VariantId,
                    Price = iv.Price,
                    SizeId = iv.SizeId
                }).ToList()
            }).ToList();

            return Ok(menuItemDtos);
        }

        // Update a MenuItem
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItemDTO updatedMenuItemDto)
        {
            var existingMenuItem = await _dbContext.MenuItems.FindAsync(id);
            if (existingMenuItem == null)
            {
                return NotFound();
            }

            existingMenuItem.ItemName = updatedMenuItemDto.ItemName;
            existingMenuItem.Price = updatedMenuItemDto.Price;
            existingMenuItem.CategoryId = updatedMenuItemDto.CategoryId;
            existingMenuItem.Type = updatedMenuItemDto.Type;
            existingMenuItem.Description = updatedMenuItemDto.Description;
            existingMenuItem.Ingredient = updatedMenuItemDto.Ingredient;
            existingMenuItem.ImageURL = updatedMenuItemDto.ImageURL;

            _dbContext.MenuItems.Update(existingMenuItem);
            await _dbContext.SaveChangesAsync();

            return Ok(existingMenuItem);
        }

        // Delete a MenuItem
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _dbContext.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _dbContext.MenuItems.Remove(menuItem);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
