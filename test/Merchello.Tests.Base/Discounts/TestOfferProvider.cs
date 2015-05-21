namespace Merchello.Tests.Base.Discounts
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Services;

    public class TestOfferProvider : OfferProviderBase 
    {
        public TestOfferProvider(IOfferSettingsService offerSettingsService)
            : base(offerSettingsService)
        {
        }

        public override Guid Key
        {
            get { return new Guid("AD4E890D-9D60-442A-A19A-6FE9EE3A1454"); }
        }
    }
}