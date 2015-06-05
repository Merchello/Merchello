namespace Merchello.Core.Chains.OfferConstraints
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The line item collection offer constraint chain.
    /// </summary>
    [OfferConstraintChainFor(typeof(ILineItemContainer), typeof(ILineItem))]
    internal class LineItemContainerOfferAttemptChain : OfferAttemptChainBase<ILineItemContainer, ILineItem>
    {
        public LineItemContainerOfferAttemptChain(IEnumerable<OfferConstraintComponentBase<ILineItemContainer>> constraints, OfferRewardComponentBase reward)
            : base(constraints, reward)
        {
        }

        protected override OfferConstraintChainTask<ILineItemContainer> ConvertConstraintToTask(OfferConstraintComponentBase<ILineItemContainer> constraint, ICustomerBase customer)
        {
            return new LineItemContainerOfferConstraintTask(constraint, customer);
        }

    }
}