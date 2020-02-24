using System.Threading.Tasks;

namespace Convey.CQRS.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync<T>(T @event) where T : class, IEvent;

        Task DispatchAsync<T>(T @event) where T : class, IEvent;
    }
}