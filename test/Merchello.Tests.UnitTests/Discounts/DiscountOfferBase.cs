namespace Merchello.Tests.UnitTests.Discounts
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core;

    public abstract class DiscountOfferBase : IDiscountOffer
    {
        protected DiscountOfferBase(IEnumerable<IDiscountConstraint> constraints)
        {
            this.Constraints = constraints;
        }

        protected IEnumerable<IDiscountConstraint> Constraints { get; private set; }


        protected abstract Attempt<IDiscountReward> ApplyReward(ILineItemContainer collection);

        public string Name { get; set; }

        public Guid DiscountProviderKey { get; set; }

        public DateTime OfferStartsDate { get; set; }

        public DateTime OfferEndsDate { get; set; }

        public bool Active { get; set; }

        public Attempt<ILineItem> TryApply(ILineItemContainer collection)
        {
            var applied = this.ApplyReward(collection);
            return applied
                       ? Attempt<ILineItem>.Succeed(new InvoiceLineItem(LineItemType.Discount, this.Name, "Sku", 1, 10))
                       : Attempt<ILineItem>.Fail();
        }
    }
}