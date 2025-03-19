namespace UserAccessSystem.Domain;

public class Group : BaseDomainObj
{
    public required string Name { get; set; }
    public required string? Description { get; set; }

    public required IEnumerable<UserGroupMembership> Users { get; set; } =
        ArraySegment<UserGroupMembership>.Empty;

    public required IEnumerable<GroupPermission> GroupPermissions { get; set; } =
        ArraySegment<GroupPermission>.Empty;
}
