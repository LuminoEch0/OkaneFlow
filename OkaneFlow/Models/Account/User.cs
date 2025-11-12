using System.ComponentModel.DataAnnotations;

namespace OkaneFlow.Models.Account
{
    public class User
    {
        public Guid UserID { get; set; } //pk

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        [StringLength(256)]
        public required string PasswordHash { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsAdmin { get; set; } = false;

    }
}
