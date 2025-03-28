using UserAccessSystem.Domain.User;

namespace UserAccessSystem.Contract.Dtos;

public class UserDto
{
    public User User { get; set; }

    public UserDto() { }

    public UserDto(User user)
    {
        this.User = user;
    }
}
