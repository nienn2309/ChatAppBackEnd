using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatAppBackE.Models
{
    public class Conversation
    {
        [Key]
        [Column("Conversation_Id")]
        public string ConversationId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Created_At")]
        public DateTime CreatedAt { get; set; }

        [Column("Current_message_time")]
        public DateTime? CurrentMessageTime { get; set; }

        public ICollection<UserConversation> UserConversations { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
