namespace ManageUsers.Domain
{
    public sealed class Region : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
