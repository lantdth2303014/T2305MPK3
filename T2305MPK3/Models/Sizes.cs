using System.ComponentModel.DataAnnotations;

namespace T2305MPK3.Models
{
    public class Sizes
    {
        [Key]
        public long SizeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SizeNumber { get; set; }
    }
}
