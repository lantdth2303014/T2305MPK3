using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
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
        public async Task<IActionResult> CreateOrderDetail([FromBody] CustOrderDetail orderDetail)
        {
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

        // Update a CustOrderDetail
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] CustOrderDetail updatedOrderDetail)
        {
            var existingOrderDetail = await _dbContext.CustOrderDetails.FindAsync(id);
            if (existingOrderDetail == null)
            {
                return NotFound();
            }

            existingOrderDetail.OrderId = updatedOrderDetail.OrderId;
            existingOrderDetail.CategoryId = updatedOrderDetail.CategoryId;
            existingOrderDetail.VariantId = updatedOrderDetail.VariantId;
            existingOrderDetail.Price = updatedOrderDetail.Price;

            _dbContext.CustOrderDetails.Update(existingOrderDetail);
            await _dbContext.SaveChangesAsync();

            return Ok(existingOrderDetail);
        }

        // Delete a CustOrderDetail
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var orderDetail = await _dbContext.CustOrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _dbContext.CustOrderDetails.Remove(orderDetail);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
