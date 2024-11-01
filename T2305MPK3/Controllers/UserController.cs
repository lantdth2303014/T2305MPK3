using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPut("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer customer)
        {
            var existingCustomer = await _dbContext.Customers.FindAsync(customer.CustomerId);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            existingCustomer.Name = customer.Name;
            existingCustomer.Address = customer.Address;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Email = customer.Email;
            existingCustomer.ImageURL = customer.ImageURL;

            _dbContext.Customers.Update(existingCustomer);
            await _dbContext.SaveChangesAsync();

            return Ok("Customer information updated successfully.");
        }

        [HttpPut("caterer")]
        [Authorize(Roles = "Caterer")]
        public async Task<IActionResult> UpdateCaterer([FromBody] Caterer caterer)
        {
            var existingCaterer = await _dbContext.Caterers.FindAsync(caterer.CatererId);
            if (existingCaterer == null)
            {
                return NotFound("Caterer not found.");
            }

            existingCaterer.Name = caterer.Name;
            existingCaterer.Address = caterer.Address;
            existingCaterer.Phone = caterer.Phone;
            existingCaterer.Email = caterer.Email;
            existingCaterer.Description = caterer.Description;
            existingCaterer.ImageURL = caterer.ImageURL;

            _dbContext.Caterers.Update(existingCaterer);
            await _dbContext.SaveChangesAsync();

            return Ok("Caterer information updated successfully.");
        }
    }
}
