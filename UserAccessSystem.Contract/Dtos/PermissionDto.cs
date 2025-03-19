using UserAccessSystem.Domain.Permissions;

namespace UserAccessSystem.Contract.Dtos;

public class PermissionDto
{
    public Permission Permission { get; set; }

    public PermissionDto() { }

    public PermissionDto(Permission permission)
    {
        this.Permission = permission;
    }
}
