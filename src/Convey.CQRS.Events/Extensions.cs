using System;
using System.Threading.Tasks;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.CQRS.Events
{
    public static class Extensions
    {
        public static IConveyBuilder AddEventHandlers(this IConveyBuilder builder)
        {
            builder.Services.Scan(s =>
                s.FromEntryAssembly()
                    .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return builder;
        }
        
        public static Task PublishAsync<TEvent>(this IBusPublisher busPublisher, TEvent @event, ICorrelationContext context) 
            where TEvent : IEvent
            => busPublisher.PublishAsync(@event, context);

        public static IBusSubscriber SubscribeEvent<TEvent>(this IBusSubscriber busSubscriber, 
            string @namespace = null, string queueName = null, Func<TEvent, ConveyException, IMessage> onError = null) 
            where TEvent : IEvent
        {
            busSubscriber.SubscribeMessage<TEvent>(async (sp, @event, ctx) =>
            {
                var commandHandler = sp.GetService<IEventHandler<TEvent>>();
                await commandHandler.HandleAsync(@event, ctx);
            }, @namespace, queueName, onError);

            return busSubscriber;
        }
    }
}