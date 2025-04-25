using Domain.Interfaces.Services;

namespace Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId => Guid.NewGuid(); 
    public string UserName => "System";
    public bool IsAuthenticated => true;
}
