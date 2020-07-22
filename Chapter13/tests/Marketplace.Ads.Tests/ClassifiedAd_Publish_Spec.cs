using System;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Xunit;

namespace Marketplace.ClassifiedAds.Tests
{
    public class ClassifiedAd_Publish_Spec
    {
        public ClassifiedAd_Publish_Spec()
            => Ad = ClassifiedAd.Create(
                ClassifiedAdId.FromGuid(Guid.NewGuid()),
                UserId.FromGuid(Guid.NewGuid())
            );

        ClassifiedAd Ad { get; }

        [Fact]
        public void Can_publish_a_valid_ad()
        {
            Ad.SetTitle(ClassifiedAdTitle.FromString("Test ad"));

            Ad.UpdateText(
                ClassifiedAdText.FromString("Please buy my stuff")
            );

            Ad.UpdatePrice(
                Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup())
            );

            Ad.RequestToPublish();

            Assert.Equal(
                ClassifiedAd.ClassifiedAdState.PendingReview,
                Ad.State
            );
        }

        [Fact]
        public void Cannot_publish_with_zero_price()
        {
            Ad.SetTitle(ClassifiedAdTitle.FromString("Test ad"));

            Ad.UpdateText(
                ClassifiedAdText.FromString("Please buy my stuff")
            );

            Ad.UpdatePrice(
                Price.FromDecimal(0.0m, "EUR", new FakeCurrencyLookup())
            );

            Assert.Throws<DomainExceptions.InvalidEntityState>(
                () => Ad.RequestToPublish()
            );
        }

        [Fact]
        public void Cannot_publish_without_price()
        {
            Ad.SetTitle(ClassifiedAdTitle.FromString("Test ad"));

            Ad.UpdateText(
                ClassifiedAdText.FromString("Please buy my stuff")
            );

            Assert.Throws<DomainExceptions.InvalidEntityState>(
                () => Ad.RequestToPublish()
            );
        }

        [Fact]
        public void Cannot_publish_without_text()
        {
            Ad.SetTitle(ClassifiedAdTitle.FromString("Test ad"));

            Ad.UpdatePrice(
                Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup())
            );

            Assert.Throws<DomainExceptions.InvalidEntityState>(
                () => Ad.RequestToPublish()
            );
        }

        [Fact]
        public void Cannot_publish_without_title()
        {
            Ad.UpdateText(
                ClassifiedAdText.FromString("Please buy my stuff")
            );

            Ad.UpdatePrice(
                Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup())
            );

            Assert.Throws<DomainExceptions.InvalidEntityState>(
                () => Ad.RequestToPublish()
            );
        }
    }
}