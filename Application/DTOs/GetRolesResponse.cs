namespace ManageUsers.Application.DTOs
{
    public class GetRolesResponse : BaseResponse
    {
        public List<RoleItem>? Items { get; set; }
    }

    public sealed class RoleItem : PaginationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NextRoleName { get; set; }
    }
}
