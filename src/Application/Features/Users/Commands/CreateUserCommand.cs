using Application.Abstractions.Messaging;
using Application.Models;

namespace Application.Features.Users.Commands;

public sealed record CreateUserCommand(string Email, string Name): ICommand<UserViewModel>, ITransactionalCommand;