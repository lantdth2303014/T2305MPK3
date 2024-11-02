using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class CustOrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public int CategoryId { get; set; }
        public int VariantId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("OrderId")]
        public CustOrder CustOrder { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
