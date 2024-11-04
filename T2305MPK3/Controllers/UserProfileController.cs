using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public UserProfileController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get current user's profile
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            // Query Customer by LoginMasterId
            var userProfile = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.LoginMasterId == userId);

            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(userProfile);
        }
    }
}
