using ChatAppBackE.DTO;
using ChatAppBackE.Models;
using AutoMapper;

namespace ChatAppBackE.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();

            // Conversation mappings
            CreateMap<Conversation, ConversationDto>();
            CreateMap<CreateConversationDto, Conversation>();

            // Message mappings
            CreateMap<Message, MessageDto>();
            CreateMap<CreateMessageDto, Message>();
        }
    }
}
