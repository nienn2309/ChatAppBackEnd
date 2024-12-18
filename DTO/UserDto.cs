namespace ChatAppBackE.DTO
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; }
    }
}
