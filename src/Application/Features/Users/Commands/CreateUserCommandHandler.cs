using Application.Abstractions.Messaging;
using Domain.Entities;
using ErrorOr;
using Persistence.Context;

namespace Application.Features.Users.Commands;

internal sealed class CreateUserCommandHandler(ApplicationDbContext _context) : ICommandHandler<CreateUserCommand, User>
{
    public async Task<ErrorOr<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(request.Email, request.Name);
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }
}
