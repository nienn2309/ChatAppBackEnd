using ChatAppBackE.Data;
using ChatAppBackE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ChatAppBackE.DTO;

namespace ChatAppBackE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        public Controller(AppDBContext appDbContext, IMapper mapper)
        {
            _dbContext = appDbContext;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = userDto.Username
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}
