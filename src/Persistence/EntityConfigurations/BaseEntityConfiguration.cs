using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Abstractions;

namespace Persistence.EntityConfigurations;

/// <summary>
/// Classe base para configurações de entidades no Entity Framework Core
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TId">Tipo do ID da entidade</typeparam>
public abstract class BaseEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Configuração básica para todas as entidades

        // Chave primária
        builder.HasKey(e => e.Id);
        
        // Propriedade Id
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();
         
        // Propriedades de auditoria
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(50);

        builder.Property(e => e.UpdatedAt);
                
        // Índice para otimizar consultas por data de criação
        builder.HasIndex(e => e.CreatedAt);

    }
}
