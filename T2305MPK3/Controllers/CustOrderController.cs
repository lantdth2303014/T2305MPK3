using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text;
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

        // Get all CustOrders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _dbContext.CustOrders
                .Include(o => o.Customer) // Include customer details
                .ToListAsync();

            return Ok(orders);
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

        // Get CustOrders by CustomerId
        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetCustOrdersByCustomerId(int customerId)
        {
            // Fetch orders for the given CustomerId
            var custOrders = await _dbContext.CustOrders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Customer) // Optionally include customer details
                .ToListAsync();

            if (!custOrders.Any())
            {
                return NotFound($"No orders found for CustomerId {customerId}.");
            }

            return Ok(custOrders);
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

        // Approve an order
        [HttpPut("{orderId}/approve")]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            var order = await _dbContext.CustOrders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            order.Status = "Approved";

            _dbContext.CustOrders.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = $"Order {orderId} approved successfully.", Status = order.Status });
        }

        // Cancel an order
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _dbContext.CustOrders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            order.Status = "Cancelled";

            _dbContext.CustOrders.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = $"Order {orderId} cancelled successfully.", Status = order.Status });
        }

        // Preparing an order
        [HttpPut("{orderId}/prepare")]
        public async Task<IActionResult> PrepOrder(int orderId)
        {
            var order = await _dbContext.CustOrders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            order.Status = "Preparing";

            _dbContext.CustOrders.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = $"Preparing order {orderId}.", Status = order.Status });
        }

        // Order ready to serve
        [HttpPut("{orderId}/ready")]
        public async Task<IActionResult> OrderSet(int orderId)
        {
            var order = await _dbContext.CustOrders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            order.Status = "Ready";

            _dbContext.CustOrders.Update(order);
            await _dbContext.SaveChangesAsync();

            // Send email to customer
            if (!string.IsNullOrEmpty(order.Email))
            {
                try
                {
                    await SendOrderReadyEmail(order);
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        Message = $"Order {orderId} is ready to serve, but email could not be sent.",
                        Status = order.Status,
                        Error = ex.Message
                    });
                }
            }

            return Ok(new { Message = $"Order {orderId} is ready to serve.", Status = order.Status });
        }

        private async Task SendOrderReadyEmail(CustOrder order)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Online Caterer", "your_email@example.com"));
            email.To.Add(new MailboxAddress(order.Name, order.Email));
            email.Subject = "Your Order is Ready!";

            // Replace placeholders in the HTML
            string emailBody = System.IO.File.ReadAllText(Path.Combine("wwwroot", "Templates", "OrderReadyTemplate.html"));
            emailBody = emailBody.Replace("{{CustomerName}}", order.Name ?? "Valued Customer");
            emailBody = emailBody.Replace("{{OrderId}}", order.OrderId.ToString());
            emailBody = emailBody.Replace("{{OrderDetails}}", GenerateOrderDetailsHtml(order));

            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("lantdth2303014@fpt.edu.vn", "agiiigqrzafxnbay");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string GenerateOrderDetailsHtml(CustOrder order)
        {
            // Generate a table of order details dynamically
            var orderDetails = _dbContext.CustOrderDetails
                .Where(d => d.OrderId == order.OrderId)
                .Include(d => d.Category)
                .ToList();

            var sb = new StringBuilder();
            foreach (var detail in orderDetails)
            {
                sb.Append($@"
                    <tr>
                        <td>{detail.Category?.CategoryName ?? "Unknown"}</td>
                        <td>{detail.VariantId}</td>
                        <td align='right'>${detail.Price:F2}</td>
                    </tr>");
            }

            return sb.ToString();
        }
    }
}
