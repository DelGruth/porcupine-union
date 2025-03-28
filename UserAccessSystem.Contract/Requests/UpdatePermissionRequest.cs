using System.ComponentModel.DataAnnotations;

namespace UserAccessSystem.Contract.Requests;

public record UpdatePermissionRequest
{
    [Required(ErrorMessage = "Permission ID is required")]
    public required Guid Id { get; init; }

    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Permission name must be between 3 and 20 characters")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(150, ErrorMessage = "Description cannot exceed 150 characters")]
    public required string Description { get; init; }

    [Required(ErrorMessage = "ReadOnly flag is required")]
    public required bool ReadOnly { get; init; }

    [Required(ErrorMessage = "WriteOnly flag is required")]
    public required bool WriteOnly { get; init; }
}