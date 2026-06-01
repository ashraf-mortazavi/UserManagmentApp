using ManageUsers.Domain;

namespace ManageUsers.Application.DTOs
{
    public class CreateTokenContext
    {
        public required User User { get; set; }
        public required bool ActiveRole { get; set; }
        public required HttpContext HttpContext { get; set; }
        public List<RolePermission>? RolePermissions { get; set; }

    }
}
