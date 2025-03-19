namespace UserAccessSystem.Domain;

public class UserPermission : BaseDomainObj
{
    public required Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required Guid GroupId { get; set; }
    public Group? Group { get; set; }
}
