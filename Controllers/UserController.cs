using ChatAppBackE.Data;
using ChatAppBackE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ChatAppBackE.DTO;

namespace ChatAppBackE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;

        public UserController(AppDBContext appDbContext, IMapper mapper)
        {
            _dbContext = appDbContext;
            _mapper = mapper;
        }

        // Add user
        [HttpPost("add")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
            if (existingUser != null)
            {
                return Ok(_mapper.Map<UserDto>(existingUser));
            }

            var newUser = _mapper.Map<User>(userDto);
            newUser.UserId = Guid.NewGuid().ToString();
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return Ok(newUser);
        }

        // Search user by name
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUser(string username)
        {
            var users = await _dbContext.Users
                .Where(u => u.Username.Contains(username))
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        // Delete User
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
