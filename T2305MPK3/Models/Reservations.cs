using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2305MPK3.Models
{
    public class Reservations
    {
        [Key]
        public int ReservationId { get; set; } // Primary key

        [Required]
        public long RestaurantId { get; set; } // Foreign key to the Restaurant table

        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; } // Navigation property to Restaurant

        [Required]
        public DateTime ReservationDate { get; set; } // Date of the reservation

        [Required]
        public TimeSpan ReservationTime { get; set; } // Time of the reservation
    }
}
