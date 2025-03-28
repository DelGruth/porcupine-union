using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Contract.Dtos;

public class UserSimpleDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public UserSimpleDto() { }

    public UserSimpleDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
    }
}