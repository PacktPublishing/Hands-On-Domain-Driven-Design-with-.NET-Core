using System;

namespace Marketplace.Domain
{
    public class Price : Money
    {
        protected Price() {}
        
        private Price(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
            : base(amount, currencyCode, currencyLookup)
        {
            if (amount < 0)
                throw new ArgumentException(
                    "Price cannot be negative",
                    nameof(amount));
        }

        internal Price(decimal amount, string currencyCode)
            : base(amount, new Currency{CurrencyCode = currencyCode, InUse = true})
        {
        }

        public new static Price FromDecimal(decimal amount, string currency,
            ICurrencyLookup currencyLookup) =>
            new Price(amount, currency, currencyLookup);
        
        public static Price NoPrice =>
            new Price
            {
                Amount = -1, 
                Currency = Currency.None
            };
    }
}