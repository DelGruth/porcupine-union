using System.ComponentModel.DataAnnotations;

namespace UserAccessSystem.Contract.Requests;

public record CreateUserRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(
        20,
        MinimumLength = 3,
        ErrorMessage = "Username must be between 3 and 20 characters"
    )]
    public required string Username { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(60, ErrorMessage = "Email cannot exceed 60 characters")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(
        60,
        MinimumLength = 1,
        ErrorMessage = "Password must be between 8 and 60 characters"
    )]
    public required string Password { get; init; }
}
