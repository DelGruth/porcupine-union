namespace UserAccessSystem.Domain;

public class UserPermission : BaseDomainObj
{
    public required Guid PermissionId { get; set; }
    public required Permission Permission { get; set; }
    public required Guid UserId { get; set; }
    public required User User { get; set; }
}
