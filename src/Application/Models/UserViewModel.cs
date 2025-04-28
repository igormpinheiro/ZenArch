namespace Application.Models;

public sealed record UserViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string CreatedBy { get; init; }
    public string CreatedAt { get; init; }
    public string? UpdatedBy { get; init; }
    public string? UpdatedAt { get; init; }
}

