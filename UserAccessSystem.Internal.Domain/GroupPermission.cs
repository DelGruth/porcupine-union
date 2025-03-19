namespace UserAccessSystem.Domain;

public class GroupPermission : BaseDomainObj
{
    public required Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
    public required Guid GroupId { get; set; }
    public Group Group { get; set; }
}
