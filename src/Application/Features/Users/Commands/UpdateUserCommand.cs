using Application.Abstractions.Messaging;
using Application.Models;

namespace Application.Features.Users.Commands;

public sealed record UpdateUserCommand(Guid Id, string Email, string Name) : ICommand<UserViewModel>;
