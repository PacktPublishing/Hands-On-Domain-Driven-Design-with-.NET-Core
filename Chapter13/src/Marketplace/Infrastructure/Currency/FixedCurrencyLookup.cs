using System.Collections.Generic;
using System.Linq;
using Marketplace.Ads.Domain.Shared;

namespace Marketplace.Infrastructure.Currency
{
    public class FixedCurrencyLookup : ICurrencyLookup
    {
        static readonly IEnumerable<Ads.Domain.Shared.Currency> Currencies =
            new[]
            {
                new Ads.Domain.Shared.Currency
                {
                    CurrencyCode = "EUR",
                    DecimalPlaces = 2,
                    InUse = true
                },
                new Ads.Domain.Shared.Currency
                {
                    CurrencyCode = "USD",
                    DecimalPlaces = 2,
                    InUse = true
                }
            };

        public Ads.Domain.Shared.Currency FindCurrency(string currencyCode)
        {
            var currency =
                Currencies.FirstOrDefault(x => x.CurrencyCode == currencyCode);
            return currency ?? Ads.Domain.Shared.Currency.None;
        }
    }
}