﻿using Microsoft.AspNetCore.Mvc;
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
    }
}