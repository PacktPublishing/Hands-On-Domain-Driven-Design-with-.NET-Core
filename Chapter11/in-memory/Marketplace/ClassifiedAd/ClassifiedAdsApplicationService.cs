using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using static Marketplace.ClassifiedAd.Contracts;

namespace Marketplace.ClassifiedAd
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly ICurrencyLookup _currencyLookup;
        private readonly IAggregateStore _store;

        public ClassifiedAdsApplicationService(
            IAggregateStore store, ICurrencyLookup currencyLookup
        )
        {
            _currencyLookup = currencyLookup;
            _store = store;
        }

        public Task Handle(object command) =>
            command switch
            {
                V1.Create cmd =>
                    HandleCreate(cmd),
                V1.SetTitle cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.SetTitle(
                            ClassifiedAdTitle
                                .FromString(cmd.Title)
                        )
                    ),
                V1.UpdateText cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.UpdateText(
                            ClassifiedAdText
                                .FromString(cmd.Text)
                        )
                    ),
                V1.UpdatePrice cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.UpdatePrice(
                            Price.FromDecimal(
                                cmd.Price,
                                cmd.Currency,
                                _currencyLookup
                            )
                        )
                    ),
                V1.RequestToPublish cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.RequestToPublish()
                    ),
                V1.Publish cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.Publish(new UserId(cmd.ApprovedBy))
                    ),
                _ => Task.CompletedTask
            };

        private async Task HandleCreate(V1.Create cmd)
        {
            if (await _store.Exists<Domain.ClassifiedAd.ClassifiedAd, 
                ClassifiedAdId>(
                new ClassifiedAdId(cmd.Id)
            ))
                throw new InvalidOperationException(
                    $"Entity with id {cmd.Id} already exists");

            var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(
                new ClassifiedAdId(cmd.Id),
                new UserId(cmd.OwnerId)
            );

            await _store.Save<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(classifiedAd);
        }

        private Task HandleUpdate(
            Guid id,
            Action<Domain.ClassifiedAd.ClassifiedAd> update
        ) =>
            this.HandleUpdate(
                _store,
                new ClassifiedAdId(id),
                update
            );
    }
}
