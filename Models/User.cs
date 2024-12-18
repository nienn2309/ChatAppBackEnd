using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatAppBackE.Models
{
    public class User
    {
        [Key]
        [Column("User_Id")]
        public string UserId { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        public ICollection<UserConversation> UserConversations { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
