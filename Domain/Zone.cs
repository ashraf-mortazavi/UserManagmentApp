namespace ManageUsers.Domain
{
    public sealed class Zone : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }


        public ICollection<Area> Areas { get; set; } = new List<Area>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
