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
    public class UserConversationController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;

        public UserConversationController(AppDBContext appDbContext, IMapper mapper)
        {
            _dbContext = appDbContext;
            _mapper = mapper;
        }

        // Controllers/UserConversationController.cs
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

        // Add a member to a conversation
        [HttpPost("{conversationId}/add-member")]
        public async Task<ActionResult> AddMember(string conversationId, AddmemberDTO addmemberDto)
        {
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
