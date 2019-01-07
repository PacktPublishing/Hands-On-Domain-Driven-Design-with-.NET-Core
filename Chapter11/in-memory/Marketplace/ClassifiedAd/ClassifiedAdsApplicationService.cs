using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;

namespace Marketplace.ClassifiedAd
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly ICurrencyLookup _currencyLookup;
        private readonly IAggregateStore _store;

        public ClassifiedAdsApplicationService(
            IAggregateStore store, ICurrencyLookup currencyLookup)
        {
            _currencyLookup = currencyLookup;
            _store = store;
        }

        public async Task Handle(object command)
        {
            switch (command)
            {
                case Contracts.V1.Create cmd:
                    if (await _store.Exists<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
                        new ClassifiedAdId(cmd.Id)))
                        throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

                    var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(
                        new ClassifiedAdId(cmd.Id),
                        new UserId(cmd.OwnerId));

                    await _store.Save<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(classifiedAd);
                    break;

                case Contracts.V1.SetTitle cmd:
                    await this.HandleUpdate<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
                        _store, new ClassifiedAdId(cmd.Id),
                        c => c.SetTitle(ClassifiedAdTitle.FromString(cmd.Title)));
                    break;

                case Contracts.V1.UpdateText cmd:
                    await this.HandleUpdate<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
                        _store, new ClassifiedAdId(cmd.Id),
                        c => c.UpdateText(ClassifiedAdText.FromString(cmd.Text)));
                    break;

                case Contracts.V1.UpdatePrice cmd:
                    await this.HandleUpdate<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
                        _store, new ClassifiedAdId(cmd.Id),
                        c => c.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.Currency, _currencyLookup)));
                    break;

                case Contracts.V1.RequestToPublish cmd:
                    await this.HandleUpdate<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
                        _store, new ClassifiedAdId(cmd.Id),
                        c => c.RequestToPublish());
                    break;

                case Contracts.V1.Publish cmd:
                    await this.HandleUpdate<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
                        _store, new ClassifiedAdId(cmd.Id),
                        c => c.Publish(new UserId(cmd.ApprovedBy)));
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Command type {command.GetType().FullName} is unknown");
            }
        }
    }
}