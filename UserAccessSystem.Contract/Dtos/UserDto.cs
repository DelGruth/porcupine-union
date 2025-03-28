using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Contract.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public IEnumerable<Guid> GroupIds { get; set; }
    public IEnumerable<Guid> PermissionIds { get; set; }

    public UserDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
        GroupIds = user.Groups?.Select(g => g.GroupId) ?? new List<Guid>();
        PermissionIds = user.UserPermissions?.Select(p => p.PermissionId) ?? new List<Guid>();
    }
}
