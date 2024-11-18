using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RestaurantId { get; set; } // Maps to Restaurant_id in the table

        [Required]
        [MaxLength(50)]
        public string RestaurantName { get; set; }

        [Required]
        public string Address { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        [Required]
        public byte Rating { get; set; }
    }
}
