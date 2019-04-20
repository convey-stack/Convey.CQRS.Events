using System.Threading.Tasks;

namespace Convey.CQRS.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<T>(T @event) where T : IEvent;
    }
}