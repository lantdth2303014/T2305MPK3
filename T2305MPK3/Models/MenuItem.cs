using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuItemNo { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        public string Description { get; set; }

        public string Ingredient { get; set; }

        public string ImageURL { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
