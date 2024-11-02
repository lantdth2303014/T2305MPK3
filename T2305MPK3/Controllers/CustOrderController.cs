using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustOrderController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CustOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create a new CustOrder
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CustOrder order)
        {
            _dbContext.CustOrders.Add(order);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // Get a specific CustOrder by OrderId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _dbContext.CustOrders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // Update a CustOrder
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] CustOrder updatedOrder)
        {
            var existingOrder = await _dbContext.CustOrders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.DeliveryDate = updatedOrder.DeliveryDate;
            existingOrder.NoOfPeople = updatedOrder.NoOfPeople;
            existingOrder.NoOfTable = updatedOrder.NoOfTable;
            existingOrder.OrderNote = updatedOrder.OrderNote;
            existingOrder.DepositCost = updatedOrder.DepositCost;
            existingOrder.TotalCost = updatedOrder.TotalCost;
            existingOrder.CustomerId = updatedOrder.CustomerId;

            _dbContext.CustOrders.Update(existingOrder);
            await _dbContext.SaveChangesAsync();

            return Ok(existingOrder);
        }

        // Delete a CustOrder
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _dbContext.CustOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _dbContext.CustOrders.Remove(order);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
