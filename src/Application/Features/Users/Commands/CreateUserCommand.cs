using Application.Abstractions.Messaging;
using Domain.Entities;

namespace Application.Features.Users.Commands;

public sealed record CreateUserCommand(string Email, string Name): ICommand<User>, ITransactionalCommand;