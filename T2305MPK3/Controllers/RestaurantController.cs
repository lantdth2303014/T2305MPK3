using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public RestaurantController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new Restaurant
        [HttpPost]
        public async Task<IActionResult> CreateRestaurant([FromBody] Restaurant restaurant)
        {
            _dbContext.Restaurants.Add(restaurant);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRestaurantById), new { id = restaurant.RestaurantId }, restaurant);
        }

        // Get a specific Restaurant by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurantById(long id)
        {
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        // Get all Restaurants
        [HttpGet]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var restaurants = await _dbContext.Restaurants.ToListAsync();
            return Ok(restaurants);
        }

        // Update a Restaurant
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(long id, [FromBody] Restaurant updatedRestaurant)
        {
            var existingRestaurant = await _dbContext.Restaurants.FindAsync(id);
            if (existingRestaurant == null)
            {
                return NotFound();
            }

            existingRestaurant.RestaurantName = updatedRestaurant.RestaurantName;
            existingRestaurant.Address = updatedRestaurant.Address;
            existingRestaurant.Description = updatedRestaurant.Description;
            existingRestaurant.Rating = updatedRestaurant.Rating;

            _dbContext.Restaurants.Update(existingRestaurant);
            await _dbContext.SaveChangesAsync();

            return Ok(existingRestaurant);
        }

        // Delete a Restaurant
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(long id)
        {
            var restaurant = await _dbContext.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            _dbContext.Restaurants.Remove(restaurant);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // Get restaurants available for a specific date and time
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRestaurants(DateTime reservationDate, TimeSpan reservationTime)
        {
            // Fetch all restaurants
            var allRestaurants = await _dbContext.Restaurants.ToListAsync();

            // Fetch reservations that conflict with the given date and time
            var conflictingReservations = await _dbContext.Reservations
                .Where(r => r.ReservationDate == reservationDate && r.ReservationTime == reservationTime)
                .Select(r => r.RestaurantId)
                .ToListAsync();

            // Filter out conflicting restaurants
            var availableRestaurants = allRestaurants
                .Where(r => !conflictingReservations.Contains(r.RestaurantId))
                .ToList();

            return Ok(availableRestaurants);
        }

        // Get all reservations
        [HttpGet("reservations")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _dbContext.Reservations
                .Include(r => r.Restaurant) // Include restaurant details
                .ToListAsync();

            return Ok(reservations);
        }

        // Create a new reservation
        [HttpPost("reserve")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservations reservation)
        {
            // Validate if the restaurant exists
            var restaurantExists = await _dbContext.Restaurants.AnyAsync(r => r.RestaurantId == reservation.RestaurantId);
            if (!restaurantExists)
            {
                return BadRequest($"Restaurant with ID {reservation.RestaurantId} does not exist.");
            }

            // Check for conflicts
            var conflictExists = await _dbContext.Reservations.AnyAsync(r =>
                r.RestaurantId == reservation.RestaurantId &&
                r.ReservationDate == reservation.ReservationDate &&
                r.ReservationTime == reservation.ReservationTime);

            if (conflictExists)
            {
                return Conflict("The restaurant is already reserved for the specified date and time.");
            }

            // Add reservation
            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllReservations), new { id = reservation.ReservationId }, reservation);
        }
    }
}
