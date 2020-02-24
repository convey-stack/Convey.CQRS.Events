using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Events.Handler;
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

        public async Task PublishAsync<T>(T @event) where T : class, IEvent
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices<IEventHandler<T>>();
                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(@event);
                }
            }
        }

        public async Task DispatchAsync<T>(T @event) where T : class, IEvent
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                Type handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
                Type wrapperType = typeof(Handler.EventHandler<>).MakeGenericType(@event.GetType());

                var handlers = scope.ServiceProvider.GetServices(handlerType);

                IEnumerable<Handler.EventHandler> wrappedHandlers = handlers.Cast<object>()
              .Select(handler => (Handler.EventHandler)Activator.CreateInstance(wrapperType, handler));

                foreach (Handler.EventHandler handler in wrappedHandlers)
                    await handler.Handle(@event);
            }
        }
    }
}