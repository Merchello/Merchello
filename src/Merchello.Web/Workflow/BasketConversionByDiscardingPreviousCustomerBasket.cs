namespace Merchello.Web.Workflow
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The basket conversion by discarding previously saved basket.
    /// </summary>
    public class BasketConversionByDiscardingPreviousCustomerBasket : BasketConversionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketConversionByDiscardingPreviousCustomerBasket"/> class.
        /// </summary>
        /// <param name="anonymousBasket">
        /// The anonymous basket.
        /// </param>
        /// <param name="customerBasket">
        /// The customer basket.
        /// </param>
        public BasketConversionByDiscardingPreviousCustomerBasket(IBasket anonymousBasket, IBasket customerBasket)
            : base(anonymousBasket, customerBasket)
        {
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <returns>
        /// The <see cref="IBasket"/>.
        /// </returns>
        public override IBasket Merge()
        {
            OnConverting();

            if (AnonymousBasket.IsEmpty)
            {
                OnConverted();
                return CustomerBasket;
            }

            CustomerBasket.Empty();

            CustomerBasket.Items.Add(AnonymousBasket.Items.Select(x => x.AsLineItemOf<ItemCacheLineItem>()));
            CustomerBasket.Save();

            OnConverted();

            return CustomerBasket;
        }
    }
}