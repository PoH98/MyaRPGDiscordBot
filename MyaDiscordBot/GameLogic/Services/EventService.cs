using Autofac;
using MyaDiscordBot.GameLogic.Events;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IEventService
    {
        IRandomEvent GetRandomEvent();
    }
    public class EventService : IEventService
    {
        public IRandomEvent GetRandomEvent()
        {
            var events = Data.Instance.Container.ComponentRegistry.Registrations.Where(x => typeof(IRandomEvent).IsAssignableFrom(x.Activator.LimitType)).Select(x => x.Activator.LimitType).Select(t => Data.Instance.Container.Resolve(t) as IRandomEvent);
            Random rnd = new Random();
            var index = rnd.Next(events.Count());
            return events.ToArray()[index];
        }
    }
}
