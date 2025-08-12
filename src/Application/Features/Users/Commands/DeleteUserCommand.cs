using Application.Abstractions.Messaging;

namespace Application.Features.Users.Commands;

public sealed record DeleteUserCommand(Guid Id) : ICommand;
