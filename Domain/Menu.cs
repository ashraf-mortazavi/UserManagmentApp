namespace ManageUsers.Domain
{
    public sealed class Menu: BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Route { get; set; }
        public string? Icon { get; set; }
        public bool IsVisible { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        public int? ParentId { get; set; }
        public Menu? Parent { get; set; }
        public ICollection<Menu> Children { get; set; } = new List<Menu>();

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

    }
}
