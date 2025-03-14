﻿using ChatAppBackE.Data;
using ChatAppBackE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ChatAppBackE.DTO;
using ChatAppBackE.Service;

namespace ChatAppBackE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;

        public ConversationController(AppDBContext appDbContext, IMapper mapper)
        {
            _dbContext = appDbContext;
            _mapper = mapper;
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

        // Get messages of a specified conversation
        [HttpGet("{conversationId}/messages")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(string conversationId)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<MessageDto>>(messages));
        }

        // Send a message to a specified conversation
        [HttpPost("{conversationId}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(string conversationId, CreateMessageDto messageDto, [FromServices] KafkaProducer kafkaproducer)
        {
            var conversation = await _dbContext.Conversations.FindAsync(conversationId);
            if (conversation == null)
            {
                return NotFound("Conversation not found");
            }

            var message = _mapper.Map<Message>(messageDto);
            message.MessageId = Guid.NewGuid().ToString();
            message.SentAt = DateTime.UtcNow;

            conversation.CurrentMessageTime = message.SentAt;

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            // Produce Kafka message
            var messagePayload = System.Text.Json.JsonSerializer.Serialize(messageDto);
            await kafkaproducer.SendMessageAsync(conversationId, messagePayload, "TestTopic");

            return Ok(_mapper.Map<MessageDto>(message));
        }
    }
}