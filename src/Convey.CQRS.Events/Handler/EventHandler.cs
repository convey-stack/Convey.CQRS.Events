using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Convey.CQRS.Events.Handler
{
    public abstract class EventHandler
    {
        public abstract Task Handle(IEvent @vent);
    }

    public class EventHandler<T> : EventHandler
        where T : class, IEvent
    {
        private readonly IEventHandler<T> _handler;

        public EventHandler(IEventHandler<T> handler)
        {
            _handler = handler;
        }

        public override async Task Handle(IEvent domainEvent)
        {
            await _handler.HandleAsync((T)domainEvent);
        }
    }
}
