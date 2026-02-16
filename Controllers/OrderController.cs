using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestroManagementSystem.DB;
using RestroManagementSystem.Models;

namespace RestroManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RestroContextDb _context;

        public OrderController(RestroContextDb context)
        {
            _context = context;
        }
        //get api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            var order = await _context.Orders.
                Include(o => o.User).
                Include(o => o.Orderdishes).
                    ThenInclude(od => od.Dish).
                    ToListAsync();
            return Ok(order);
        }
        //get/api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.
                Include(o => o.User).
                Include(o => o.Orderdishes).
                    ThenInclude(od => od.Dish).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Order id is not found");
            }
            return Ok(order);
        }

        //GET:api/order/user/5
        [HttpGet("user/{userid}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(int userid)
        {
            var userexist = await _context.Orders.AnyAsync(u => u.Id == userid);
            if (userexist == null)
            {
                return NotFound(new { msg = "user not found" });
            }
            var orders = await _context.Orders.Include(o => o.Orderdishes)
                .ThenInclude(od => od.Dish)
                .Where(o => o.UserId == userid)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders;
        }
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByStatus(string status)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Orderdishes)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.Status != null && o.Status.ToLower() == status.ToLower())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {

            var userExists = await _context.Users.AnyAsync(u => u.Id == order.UserId);
            if (!userExists)
            {
                return BadRequest(new { msg = "user not found" });
            }
            if (order.Orderdishes == null || !order.Orderdishes.Any())
            {
                return BadRequest(new { msg = "Order must contain at least one dish" });
            }
            order.OrderDate = DateTime.Now;
            if (string.IsNullOrEmpty(order.Status))
            {
                order.Status = "Pending";
            }

            decimal totalAmount = 0;
            //process each dish in order 
            foreach (var orderDish in order.Orderdishes)
            {
                var dish = await _context.dishes.FindAsync(orderDish.DishId);
                if (dish == null)
                {
                    return BadRequest(new { message = $"Dish with id {orderDish.DishId} not found" });
                }
                if (dish.IsAvailable == false)
                {
                    return BadRequest(new { message = $"Dish '{dish.Name}' is not available" });
                }
                //calculate amount for the dish
                orderDish.UnitPriceAtOrder = dish.Price;
                orderDish.Subtotal = dish.Price * orderDish.Quantity;
                orderDish.ItemTotal = orderDish.Subtotal - orderDish.ItemDiscount;
                totalAmount += orderDish.ItemTotal;
            }
            //calculate total amount
            order.TotalAmount = totalAmount;
            decimal amountAfterDiscount = totalAmount - order.DiscountAmount;
            //calculate tax amount
            if (order.TaxAmount == 0)
            {
                order.TaxAmount = amountAfterDiscount * 0.05m;
            }
            order.FinalAmount = amountAfterDiscount + order.TaxAmount;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var createdOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Orderdishes)
                    .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, createdOrder);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("no order found");
            }
            order.Status = status;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order status updated successfully", order });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
            .Include(o => o.Orderdishes)
            .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }
            // Only allow deletion of pending orders
            if (order.Status?.ToLower() != "pending")
            {
                return BadRequest(new { message = "Only pending orders can be deleted" });
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order deleted successfully" });
        }
    }
}