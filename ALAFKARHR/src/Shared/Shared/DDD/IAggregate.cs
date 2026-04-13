using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DDD
{

    public interface IAggregate<T>: IAggregate, IEntity<T>
    {

    }
    public interface IAggregate:IAggregateRoot,IEntity
    {
        
    }
    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvents();
    }
}
