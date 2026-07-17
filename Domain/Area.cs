namespace ManageUsers.Domain
{
    public sealed class Area : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int ZoneId { get; set; }
        public Zone Zone { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
