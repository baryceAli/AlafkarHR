
using MediatR;

namespace Shared.DDD
{
    public interface IDomainEvent: INotification
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccuredOn=>DateTime.Now;
        public string EventType => GetType().AssemblyQualifiedName!;
    }
}
