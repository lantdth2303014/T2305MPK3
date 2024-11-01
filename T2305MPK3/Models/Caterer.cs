using System.ComponentModel.DataAnnotations;

namespace T2305MPK3.Models
{
    public class Caterer
    {
        [Key]
        public int CatererId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public string? Description { get; set; }

        public string? ImageURL { get; set; }
    }
}
