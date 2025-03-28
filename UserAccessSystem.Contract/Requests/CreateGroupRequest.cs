using System.ComponentModel.DataAnnotations;

namespace UserAccessSystem.Contract.Requests;

public record CreateGroupRequest
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(
        20,
        MinimumLength = 3,
        ErrorMessage = "Group name must be between 3 and 20 characters"
    )]
    public required string Name { get; init; }

    [StringLength(150, ErrorMessage = "Description cannot exceed 150 characters")]
    public string? Description { get; init; }
}
