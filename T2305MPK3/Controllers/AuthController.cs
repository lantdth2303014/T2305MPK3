using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BCrypt.Net;
using T2305MPK3.Data;
using T2305MPK3.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer(UserRegister userRegister)
        {
            if (await _dbContext.LoginMasters.AnyAsync(u => u.Username == userRegister.Username))
            {
                return Conflict("Username already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);

            var newUser = new LoginMaster
            {
                Username = userRegister.Username,
                Password = hashedPassword,
                Role = "Customer"
            };

            _dbContext.LoginMasters.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Add the user to the Customer table with reference to LoginMasterId
            var newCustomer = new Customer
            {
                Name = userRegister.Username,
                Email = userRegister.Username,
                LoginMasterId = newUser.UserId  // Link to LoginMaster
            };

            _dbContext.Customers.Add(newCustomer);
            await _dbContext.SaveChangesAsync();

            return Ok("Customer registered successfully.");
        }

        [HttpPost("register-caterer")]
        public async Task<IActionResult> RegisterCaterer(UserRegister userRegister)
        {
            if (await _dbContext.LoginMasters.AnyAsync(u => u.Username == userRegister.Username))
            {
                return Conflict("Username already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);

            var newUser = new LoginMaster
            {
                Username = userRegister.Username,
                Password = hashedPassword,
                Role = "Caterer"
            };

            _dbContext.LoginMasters.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Add the user to the Caterer table.
            var newCaterer = new Caterer
            {
                Name = userRegister.Username,
                Email = userRegister.Username
            };

            _dbContext.Caterers.Add(newCaterer);
            await _dbContext.SaveChangesAsync();

            return Ok("Caterer registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            var user = await _dbContext.LoginMasters
                .SingleOrDefaultAsync(u => u.Username == userLogin.Username);

            if (user is not null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                var token = GenerateJwtToken(user.UserId, user.Username, user.Role); // Pass UserId here
                return Ok(new { token });
            }

            return Unauthorized();
        }

        // Protected endpoint, accessible only by users with the "Caterer" role
        [HttpGet("secure-data")]
        [Authorize(Roles = "Caterer")]
        public IActionResult GetSecureData()
        {
            return Ok("This is protected data for Caterers only.");
        }

        private string GenerateJwtToken(int userId, string username, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: new[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Add UserId claim
                    new System.Security.Claims.Claim("sub", username),
                    new System.Security.Claims.Claim("role", role)
                },
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public record UserLogin(string Username, string Password);
    public record UserRegister(string Username, string Password);
}
