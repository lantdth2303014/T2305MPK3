using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using System.Text;
using T2305MPK3.Data;
using T2305MPK3.Models;

namespace T2305MPK3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public MailController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("invoice/{orderId}")]
        public async Task<IActionResult> SendInvoiceEmail(int orderId)
        {
            // Fetch the order with details
            var order = await _dbContext.CustOrders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            // Fetch order details
            var orderDetails = await _dbContext.CustOrderDetails
                .Where(d => d.OrderId == orderId)
                .Include(d => d.Category)
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return BadRequest($"No details found for Order ID {orderId}.");
            }

            // Send the email
            if (!string.IsNullOrEmpty(order.Email))
            {
                try
                {
                    await SendInvoiceEmail(order, orderDetails);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = $"Failed to send invoice email for Order {orderId}.",
                        Error = ex.Message
                    });
                }
            }

            return Ok(new { Message = $"Invoice email sent successfully for Order {orderId}." });
        }

        private async Task SendInvoiceEmail(CustOrder order, List<CustOrderDetail> orderDetails)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Online Caterer", "your_email@example.com"));
            email.To.Add(new MailboxAddress(order.Name ?? "Customer", order.Email));
            email.Subject = "Your Order Invoice";

            // Load the HTML template and populate dynamic data
            string emailBody = System.IO.File.ReadAllText(Path.Combine("wwwroot", "Templates", "InvoiceTemplate.html"));
            emailBody = emailBody.Replace("{{CustomerName}}", order.Name ?? "Valued Customer");
            emailBody = emailBody.Replace("{{OrderId}}", order.OrderId.ToString());
            emailBody = emailBody.Replace("{{OrderDate}}", order.OrderDate.ToString("yyyy-MM-dd"));
            emailBody = emailBody.Replace("{{DeliveryDate}}", order.DeliveryDate.ToString("yyyy-MM-dd"));
            emailBody = emailBody.Replace("{{TotalCost}}", $"${order.TotalCost:F2}");
            emailBody = emailBody.Replace("{{OrderDetails}}", GenerateOrderDetailsHtml(orderDetails));

            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("lantdth2303014@fpt.edu.vn", "agiiigqrzafxnbay");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string GenerateOrderDetailsHtml(List<CustOrderDetail> orderDetails)
        {
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
