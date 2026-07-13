using ManageUsers.Domain;

namespace ManageUsers.Application.DTOs
{
    public class GetUserByIdResponse : BaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string NationalCode { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PostalCode { get; set; }
        public string? PersonalCode { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public bool Enabled { get; set; }
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Setad;
        public int? OrganizationId { get; set; }
        public int? AreaId { get; set; }
        public int? RegionId { get; set; }
        public List<string> UserRoleIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
