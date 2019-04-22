using Convey.CQRS.Events.Dispatchers;
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
        
        public static IConveyBuilder AddInMemoryEventDispatcher(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<IEventDispatcher, EventDispatcher>();
            return builder;
        }
    }
}