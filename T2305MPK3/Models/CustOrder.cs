using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class CustOrder
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        public int CustomerId { get; set; }
        public long Restaurant_id { get; set; }

        [Required]
        public int NoOfPeople { get; set; }

        [Required]
        public int NoOfTable { get; set; }

        public string OrderNote { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal? DepositCost { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal? TotalCost { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
