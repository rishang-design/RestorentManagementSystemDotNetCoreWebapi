using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestroManagementSystem.DB;
using RestroManagementSystem.Models;

namespace RestroManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {

        private readonly RestroContextDb _context;
        public DishController(RestroContextDb context)
        {
            _context = context;
        }
        [HttpGet("getdish")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDish()
        {
            return await _context.dishes.ToListAsync();
            //var dish = await _context.dishes.ToListAsync();
            //return Ok(dish);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            var dish = await _context.dishes.FindAsync(id);
            if(dish == null)
            {
                return NotFound("user not found");
            }
            return dish;
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAvailableDish()
        {
            return await _context.dishes.Where(d => d.IsAvailable == true).ToListAsync();
        }

        [HttpGet("Course/{course}")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetCourse(string course)
        {
            var dishes = await _context.dishes.Where(c => c.Course != null && c.Course.ToLower()==course.ToLower()).ToListAsync();

            if (!dishes.Any())
            {
                return NotFound(new { message = $"No dishes found for course:{course}" });
            }
            return dishes;
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Dish>>> SearchName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { message = "Search term is required" });
            }
            var dishes = await _context.dishes.Where(d => d.Name.ToLower().Contains(name.ToLower())).ToListAsync();
            return dishes;
        }
        
        [HttpPost]
        public async Task<ActionResult<Dish>> CreateDish(Dish dish)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dishes = await _context.dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dish>> UpdateDish(int id , Dish dish)
        {
            if(id!= dish.Id)
            {
                return BadRequest("id mismatch");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingDish = await _context.dishes.FindAsync(id);
            if(existingDish == null)
            {
                return NotFound("Dish not found");
            }
            existingDish.Name = dish.Name;
            existingDish.Description = dish.Description;
            existingDish.Price = dish.Price;
            existingDish.Unit = dish.Unit;
            existingDish.Course = dish.Course;
            existingDish.IsAvailable = dish.IsAvailable;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Dish updated successfully", dish = existingDish });
        }

        [HttpPatch("availability/{id}")]
        public async Task<IActionResult> UpdateDishAvailability(int id ,[FromBody] bool isAvailable)
        {
            var dish = await _context.dishes.FindAsync(id);
            if(dish == null)
            {
                return NotFound("Dish not found");
            }
            dish.IsAvailable = isAvailable;
            //await _context.dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Dish availability updated", dish });
        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<Dish>> DeleteDish(int id)
        {
            var dish = await _context.dishes.FindAsync(id);
            if(dish == null)
            {
                return NotFound("Dis item not found");
            }
            _context.dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return Ok(new { message = "dish is deleted successfuly", deleteditem = dish});
        }
    }
}
