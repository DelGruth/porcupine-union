using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Contract.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<GroupSimpleDto>? Groups { get; set; }
    public IEnumerable<PermissionDto>? Permissions { get; set; }

    public UserDto() { }

    public UserDto(User user)
    {
        if (user == null)
            return;

        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
        Groups = user
            .Groups?.Where(g => g?.Group != null && !g.IsDeleted)
            .Select(g => new GroupSimpleDto(g.Group));
        Permissions = user
            .UserPermissions?.Where(p => p?.Permission != null && !p.IsDeleted)
            .Select(p => new PermissionDto(p.Permission));
    }
}
