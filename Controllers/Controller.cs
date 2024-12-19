using ChatAppBackE.Data;
using ChatAppBackE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ChatAppBackE.DTO;

namespace ChatAppBackE.Controllers
{
    [Route("api[controller]")]
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
        //Add user
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
        //Search user by name
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUser(string username)
        {
            var users = await _dbContext.Users
                .Where(u => u.Username.Contains(username))
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }
        //Delete User
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
        // Create Conversation
        [HttpPost("create-conversation")]
        public async Task<ActionResult<ConversationDto>> CreateConversation(CreateConversationDto conversationDto)
        {
            var conversation = _mapper.Map<Conversation>(conversationDto);
            conversation.ConversationId = Guid.NewGuid().ToString();
            conversation.CreatedAt = DateTime.UtcNow;

            conversation.UserConversations = new List<UserConversation>
            {
                new UserConversation
                {
                    UserId = conversationDto.OwnerId,
                    ConversationId = conversation.ConversationId,
                    Role = UserRole.OWNER
                }
            };

            _dbContext.Conversations.Add(conversation);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateConversation), new { id = conversation.ConversationId }, _mapper.Map<ConversationDto>(conversation));
        }

        //Get messages of a specified conversation
        [HttpGet("{conversationId}/messages")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(string conversationId, int page = 1, int pageSize = 10)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<MessageDto>>(messages));
        }

        //Send a message to a specified conversation
        [HttpPost("{conversationId}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(string conversationId, CreateMessageDto messageDto)
        {
            var conversation = await _dbContext.Conversations.FindAsync(conversationId);
            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var message = _mapper.Map<Message>(messageDto);
            message.MessageId = Guid.NewGuid().ToString();
            message.SentAt = DateTime.UtcNow;

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<MessageDto>(message));
        }
        //Get list of conversations of a specified user
        [HttpGet("{userId}/conversations")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetUserConversations(string userId, string? filter = null)
        {
            var conversations = _dbContext.UserConversations
                .Include(uc => uc.Conversation)
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Conversation);

            if (!string.IsNullOrEmpty(filter))
            {
                conversations = conversations.Where(c => c.Name.Contains(filter));
            }

            var conversationList = await conversations.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ConversationDto>>(conversationList));
        }
        //Add a member to a conversation
        [HttpPost("{conversationId}/add-member")]
        public async Task<ActionResult> AddMember(string conversationId, AddmemberDTO addmemberDto)
        {
            //check conversation, user if the user is already a member of the conversation
            var conversation = await _dbContext.Conversations.FindAsync(conversationId);
            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }
            var user = await _dbContext.Users.FindAsync(addmemberDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var existingMembership = await _dbContext.UserConversations.FirstOrDefaultAsync(uc => uc.ConversationId == conversationId && uc.UserId == addmemberDto.UserId);
            if (existingMembership != null)
            {
                return BadRequest("User is already a member of this conversation");
            }
            var userConversation = new UserConversation
            {
                UserId = addmemberDto.UserId,
                ConversationId = conversationId,
                Role = UserRole.MEMBER
            };
            _dbContext.UserConversations.Add(userConversation);
            await _dbContext.SaveChangesAsync();

            return Ok("User successfully added to the conversation");
        }
    }
}
