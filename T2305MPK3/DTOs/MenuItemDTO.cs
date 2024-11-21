using System.Collections.Generic;

namespace T2305MPK3.DTOs
{
    public class MenuItemDTO
    {
        public int MenuItemNo { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string? Ingredient { get; set; }
        public string ImageURL { get; set; }
        public List<ItemVariantDTO>? ItemVariants { get; set; }
        public CategoryDTO? Category { get; set; }
    }

    public class ItemVariantDTO
    {
        public long VariantId { get; set; }
        public decimal Price { get; set; }
        public long SizeId { get; set; }
        public int MenuItemNo { get; set; }
        public string MenuItemName { get; set; }
        public string SizeNumber { get; set; }
    }

    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
