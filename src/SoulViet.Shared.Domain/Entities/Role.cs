namespace SoulViet.Shared.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}