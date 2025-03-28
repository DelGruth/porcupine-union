using UserAccessSystem.Domain.Group;

namespace UserAccessSystem.Contract.Dtos;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<UserSimpleDto>? Users { get; set; }
    public IEnumerable<PermissionDto>? Permissions { get; set; }
    public int UserCount => Users?.Count() ?? 0;

    public GroupDto() { }

    public GroupDto(Group group)
    {
        Id = group.Id;
        Name = group.Name;
        Description = group.Description;
        Users = group.Users?.Where(u => !u.IsDeleted).Select(u => new UserSimpleDto(u.User));
        Permissions = group
            .GroupPermissions?.Where(p => !p.IsDeleted)
            .Select(p => new PermissionDto(p.Permission));
    }
}
