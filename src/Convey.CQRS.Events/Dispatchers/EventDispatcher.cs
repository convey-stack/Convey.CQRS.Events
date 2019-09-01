using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.CQRS.Events.Dispatchers
{
    internal sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public EventDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        
        public Task PublishAsync<T>(T @event) where T : class, IEvent
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IEventHandler<T>>();
                if (handler is null)
                {
                    throw new InvalidOperationException($"Event handler for: '{@event}' was not found.");
                }
                
                return handler.HandleAsync(@event);
            }
        }
    }
}