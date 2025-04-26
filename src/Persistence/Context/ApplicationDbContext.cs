using Domain.Entities;
using Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;

namespace Persistence.Context;

/// <summary>
/// DbContext da aplicação
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.CreatedBy = _currentUserService.UserName;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.Now;
                    entry.Entity.UpdatedBy = _currentUserService.UserName;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
