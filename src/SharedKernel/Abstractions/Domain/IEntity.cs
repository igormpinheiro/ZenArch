namespace SharedKernel.Abstractions.Domain;

public interface IEntity
{
    //Guid Id { get; init; }
    string CreatedBy { get; set; }
    DateTime CreatedAt { get; set; }
    string? UpdatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    List<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();

    //object GetId();
}

// public interface IEntity
// {
//     object[] GetKeys();
// }

public interface IEntity<out TId> : IEntity
{
    TId Id { get; }
}