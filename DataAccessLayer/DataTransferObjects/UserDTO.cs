namespace DataAccessLayer.DataTransferObjects
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Default Role
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
