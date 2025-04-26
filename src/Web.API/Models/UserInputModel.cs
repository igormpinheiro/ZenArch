namespace Web.API.Models;

public sealed record UserInputModel(string Email, string Name);
public sealed record UserViewModel(string Id, string Email, string Name);
