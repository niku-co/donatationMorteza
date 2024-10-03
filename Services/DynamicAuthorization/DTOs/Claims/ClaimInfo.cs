namespace Services.DynamicAuthorization.DTOs.Claims;

public record ClaimInfo
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public bool Selected { get; set; }
}
