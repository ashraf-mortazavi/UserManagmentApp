using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageUsers.Domain
{
    public class ApplicationUserToken
    {
        public string HashToken { get; set; }

        public DateTime TokenExpirationDate { get; set; }
        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpirationDate { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
