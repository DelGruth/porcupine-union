using UserAccessSystem.Domain.Permissions;

namespace UserAccessSystem.Contract.Dtos;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool ReadOnly { get; set; }
    public bool WriteOnly { get; set; }
    public IEnumerable<Guid> GroupIds { get; set; }
    public IEnumerable<Guid> UserIds { get; set; }

    public PermissionDto(Permission permission)
    {
        Id = permission.Id;
        Name = permission.Name;
        Description = permission.Description;
        ReadOnly = permission.ReadOnly;
        WriteOnly = permission.WriteOnly;
        GroupIds = permission.GroupPermissions?.Select(gp => gp.GroupId) ?? new List<Guid>();
        UserIds = permission.UserPermissions?.Select(up => up.UserId) ?? new List<Guid>();
    }
}
