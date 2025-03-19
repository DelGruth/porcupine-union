namespace UserAccessSystem.Contract.Requests;

public record CreatePermissionRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool ReadOnly { get; set; }
    public required bool WriteOnly { get; set; }
}
