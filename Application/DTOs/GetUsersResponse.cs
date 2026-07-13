namespace ManageUsers.Application.DTOs
{
    public class GetUsersResponse
    {
        public List<UserDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public string? FailedResult { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
