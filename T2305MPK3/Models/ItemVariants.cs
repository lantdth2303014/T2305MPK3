using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class ItemVariants
    {
        [Key]
        public long VariantId { get; set; }

        public int MenuItemNo { get; set; }  // Changed to int to match MenuItem
        public long SizeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("MenuItemNo")]
        public MenuItem MenuItem { get; set; }

        [ForeignKey("SizeId")]
        public Sizes Size { get; set; }
    }
}
