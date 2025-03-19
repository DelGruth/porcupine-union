namespace UserAccessSystem.Domain;

public class GroupPermission : BaseDomainObj
{
    public required Guid PermissionId { get; set; }
    public required Permission Permission { get; set; }
    public required Guid GroupId { get; set; }
    public required Group Group { get; set; }
}
