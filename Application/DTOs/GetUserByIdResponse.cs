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
        public bool Enabled { get; set; }
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Setad;
        public int? AreaId { get; set; }
        public int? ZoneId { get; set; }
        public string? AreaName { get; set; }
        public string? ZoneName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthDateShamsi { get; set; }
        public string? AvatarUrl { get; set; }
        public string RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
