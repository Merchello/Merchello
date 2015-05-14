namespace Merchello.Tests.UnitTests.Discounts
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    public interface IDiscountOffer
    {
        string Name { get; set; }

        Guid DiscountProviderKey { get; set; }

        DateTime OfferStartsDate { get; set; }

        DateTime OfferEndsDate { get; set; }

        bool Active { get; set; }

        Attempt<ILineItem> TryApply(ILineItemContainer collection);
    }
}