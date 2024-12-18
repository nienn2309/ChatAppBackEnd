namespace ChatAppBackE.DTO
{
    public class ConversationDto
    {
        public string ConversationId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateConversationDto
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }
    }
}
