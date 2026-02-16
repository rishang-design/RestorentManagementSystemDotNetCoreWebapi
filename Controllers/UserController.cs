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
    public class UserController : ControllerBase
    {
        private readonly RestroContextDb _context;
        public UserController(RestroContextDb context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> RegisterUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }

            // Check for null
            if (user == null)
            {
                return BadRequest("User data is required.");
            }

            var existinguser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existinguser != null)
            {
                return Conflict("You are already registered");
            }

            var newUser = new User
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password, // TODO: Hash this password!
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // Use async version

            // Don't return password
            newUser.Password = string.Empty;
            return CreatedAtAction(nameof(RegisterUser), new { id = newUser.Id }, newUser);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Email and password are required.");

            // Always use await when using EF Core async methods
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);

            if (existingUser == null)
                return Unauthorized("Invalid email or password.");

            // Hide password before returning
            existingUser.Password = string.Empty;

            return Ok(existingUser);
        }
    }
}
