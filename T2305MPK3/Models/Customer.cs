using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        // Reference to LoginMaster table
        public int? LoginMasterId { get; set; }
        [ForeignKey("LoginMasterId")]
        public LoginMaster LoginMaster { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public string? ImageURL { get; set; }
    }
}
