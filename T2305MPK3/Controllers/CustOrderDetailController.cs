using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.DTOs;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustOrderDetailController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CustOrderDetailController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new CustOrderDetail
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail([FromBody] CustOrderDetailDTO orderDetailDto)
        {
            // Check if the Order and Category exist
            var orderExists = await _dbContext.CustOrders.AnyAsync(o => o.OrderId == orderDetailDto.OrderId);
            var categoryExists = await _dbContext.Categories.AnyAsync(c => c.CategoryId == orderDetailDto.CategoryId);

            if (!orderExists)
            {
                return BadRequest("The specified order does not exist.");
            }

            if (!categoryExists)
            {
                return BadRequest("The specified category does not exist.");
            }

            // Map DTO to CustOrderDetail model
            var orderDetail = new CustOrderDetail
            {
                OrderId = orderDetailDto.OrderId,
                CategoryId = orderDetailDto.CategoryId,
                VariantId = orderDetailDto.VariantId,
                Price = orderDetailDto.Price
            };

            _dbContext.CustOrderDetails.Add(orderDetail);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderDetailById), new { id = orderDetail.OrderDetailId }, orderDetail);
        }

        // Get a specific CustOrderDetail by OrderDetailId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetailById(int id)
        {
            var orderDetail = await _dbContext.CustOrderDetails
                .Include(od => od.CustOrder)
                .Include(od => od.Category)
                .FirstOrDefaultAsync(od => od.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return Ok(orderDetail);
        }

        // Get CustOrderDetails by OrderId
        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetCustOrderDetailsByOrderId(int orderId)
        {
            // Fetch order details for the given OrderId
            var orderDetails = await _dbContext.CustOrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Category) // Optionally include Category details
                .Include(od => od.CustOrder) // Optionally include CustOrder details
                .ToListAsync();

            if (!orderDetails.Any())
            {
                return NotFound($"No order details found for OrderId {orderId}.");
            }

            return Ok(orderDetails);
        }
    }
}
