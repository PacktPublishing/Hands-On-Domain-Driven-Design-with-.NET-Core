using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Ads.ClassifiedAds
{
    public class ClassifiedAdsCommandService : ApplicationService<ClassifiedAd>
    {
        public ClassifiedAdsCommandService(
            IAggregateStore store,
            ICurrencyLookup currencyLookup,
            UploadFile uploader) : base(store)
        {
            CreateWhen<V1.Create>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (cmd, id) => ClassifiedAd.Create(
                    ClassifiedAdId.FromGuid(id), 
                    UserId.FromGuid(cmd.OwnerId)
                )
            );

            UpdateWhen<V1.ChangeTitle>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (ad, cmd)
                    => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))
            );

            UpdateWhen<V1.UpdateText>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (ad, cmd)
                    => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text))
            );

            UpdateWhen<V1.UpdatePrice>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (ad, cmd) => ad.UpdatePrice(
                    Price.FromDecimal(
                        cmd.Price, cmd.Currency ?? "EUR", currencyLookup
                    )
                )
            );

            UpdateWhen<V1.RequestToPublish>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (ad, cmd) => ad.RequestToPublish()
            );

            UpdateWhen<V1.Publish>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (ad, cmd) => ad.Publish(UserId.FromGuid(cmd.ApprovedBy))
            );

            UpdateWhen<V1.Delete>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                (ad, cmd) => ad.Delete()
            );

            UpdateWhen<V1.UploadImage>(
                cmd => ClassifiedAdId.FromGuid(cmd.Id),
                async (ad, cmd) => ad.AddPicture(
                    await uploader(cmd.Image), new PictureSize(2000, 2000)
                )
            );
        }
    }

    public delegate Task<string> UploadFile(string file);
}