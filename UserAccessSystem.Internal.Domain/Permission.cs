namespace UserAccessSystem.Domain;

public class Permission : BaseDomainObj
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required IEnumerable<GroupPermission> GroupPermissions { get; set; } =
        ArraySegment<GroupPermission>.Empty;

    public IEnumerable<UserPermission>? UserPermissions { get; set; } =
        ArraySegment<UserPermission>.Empty;
}
