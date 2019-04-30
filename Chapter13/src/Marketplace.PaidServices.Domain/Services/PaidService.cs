using System;
using System.Linq;

namespace Marketplace.PaidServices.Domain.Services
{
    public abstract class PaidService
    {
        public static readonly PaidService[] AvailableServices =
        {
            new ShowOnTop(),
            new LargeCard(),
            new ElevatedCard(),
            new DarkCard()
        };

        public enum ServiceType
        {
            ShowOnTop, LargeCard, ElevatedCard, DarkCard, Unknown
        }

        public double Price { get; private set; }
        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
        public string Description { get; private set; }
        public object Attributes { get; private set; }
        public ServiceType Type { get; private set; }

        public class ShowOnTop : PaidService
        {
            public ShowOnTop()
            {
                Type = ServiceType.ShowOnTop;
                Price = 10;
                Duration = TimeSpan.FromDays(7);
                Description = "Show on top of the list for seven days";
            }
        }

        public class LargeCard : PaidService
        {
            public LargeCard()
            {
                Type = ServiceType.LargeCard;
                Price = 5;
                Description = "Show as a large card";
                Attributes = new {flex = 6};
            }
        }

        public class ElevatedCard : PaidService
        {
            public ElevatedCard()
            {
                Type = ServiceType.ElevatedCard;
                Price = 3;
                Description = "Show elevated";
                Attributes = new {elevated = 8};
            }
        }

        public class DarkCard : PaidService
        {
            public DarkCard()
            {
                Type = ServiceType.DarkCard;
                Price = 3;
                Description = "Use dak theme";
                Attributes = new {dark = true};
            }
        }

        public class Unknown : PaidService
        {
            public Unknown() => Type = ServiceType.Unknown;
        }

        internal static PaidService Find(ServiceType type)
            => AvailableServices.First(x => x.Type == type);

        public static PaidService Find(string serviceType)
            => Find(ParseString(serviceType));

        internal static ServiceType ParseString(string serviceType)
            => Enum.TryParse<ServiceType>(serviceType, out var type)
                ? type
                : ServiceType.Unknown;
    }
}