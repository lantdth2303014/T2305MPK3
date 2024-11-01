using System.ComponentModel.DataAnnotations;

namespace T2305MPK3.Models
{
    public class LoginMaster
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [MaxLength(10)]
        public string Role { get; set; }
    }
}
