namespace Merchello.Tests.Base.Offers
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;

    public class TestOffer : OfferBase 
    {
        public TestOffer(IOfferSettings settings)
            : base(settings)
        {
        }
    }
}