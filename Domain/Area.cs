namespace ManageUsers.Domain
{
    public sealed class Area : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Region> Regions { get; set; } = new List<Region>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
