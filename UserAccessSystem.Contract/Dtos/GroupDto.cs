using UserAccessSystem.Domain.Group;

namespace UserAccessSystem.Contract.Dtos;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserCount { get; set; }
    public IEnumerable<Guid> UserIds { get; set; } = new List<Guid>();
    public IEnumerable<Guid> PermissionIds { get; set; } = new List<Guid>();

    public GroupDto() { }

    public GroupDto(Group group)
    {
        Id = group.Id;
        Name = group.Name;
        Description = group.Description;
        UserCount = group.Users?.Count() ?? 0;
        UserIds = group.Users?.Select(u => u.UserId) ?? new List<Guid>();
        PermissionIds = group.GroupPermissions?.Select(gp => gp.PermissionId) ?? new List<Guid>();
    }
}
