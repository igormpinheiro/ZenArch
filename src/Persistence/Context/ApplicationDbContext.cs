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
   
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
