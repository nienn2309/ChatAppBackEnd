using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatAppBackE.Models
{
    public class Message
    {
        [Key]
        [Column("Message_Id")]
        public string MessageId { get; set; }

        [Column("Conversation_Id")]
        public string ConversationId { get; set; }

        [Column("User_Id")]
        public string UserId { get; set; }

        [Column("Content")]
        public string Content { get; set; }

        [Column("Sent_At")]
        public DateTime SentAt { get; set; }

        // Navigation properties
        public Conversation Conversation { get; set; }
        public User User { get; set; }
    }
}
