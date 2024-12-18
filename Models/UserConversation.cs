using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAppBackE.Models
{
    public enum UserRole
    {
        OWNER,
        MEMBER
    }

    public class UserConversation
    {
        [Column("User_Id")]
        public string UserId { get; set; }

        [Column("Conversation_Id")]
        public string ConversationId { get; set; }

        [Column("Role")]
        public UserRole Role { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Conversation Conversation { get; set; }
    }
}
