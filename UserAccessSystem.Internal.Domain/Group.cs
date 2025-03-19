namespace UserAccessSystem.Domain;

public class Group : BaseDomainObj
{
    public required string Name { get; set; }
    public required string? Description { get; set; }

    public IEnumerable<UserGroupMembership> Users { get; set; } =
        ArraySegment<UserGroupMembership>.Empty;

    public IEnumerable<GroupPermission> GroupPermissions { get; set; } =
        ArraySegment<GroupPermission>.Empty;
}
