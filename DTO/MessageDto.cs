namespace ChatAppBackE.DTO
{
    public class MessageDto
    {
        public string MessageId { get; set; }
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class CreateMessageDto
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
    }
}
