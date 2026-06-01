using Microsoft.AspNetCore.Identity;

namespace ManageUsers.Domain
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public  virtual required User User { get; set; }
        public virtual required Role Role { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }= DateTime.UtcNow;
    }
}
