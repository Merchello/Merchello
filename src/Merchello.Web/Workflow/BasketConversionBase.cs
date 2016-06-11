namespace Merchello.Web.Workflow
{
    using Merchello.Core;
    using Merchello.Core.Events;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// The base basket conversion.
    /// </summary>
    public abstract class BasketConversionBase : IBasketConversionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketConversionBase"/> class.
        /// </summary>
        /// <param name="anonymousBasket">
        /// The anonymousBasket.
        /// </param>
        /// <param name="customerBasket">
        /// The customerBasket.
        /// </param>
        internal BasketConversionBase(IBasket anonymousBasket, IBasket customerBasket)
        {
            Mandate.ParameterNotNull(anonymousBasket, "anonymousBasket");
            Mandate.ParameterNotNull(customerBasket, "customerBasket");

            AnonymousBasket = anonymousBasket;
            CustomerBasket = customerBasket;
        }

        /// <summary>
        /// Occurs before conversion.
        /// </summary>
        public static event TypedEventHandler<BasketConversionBase, ConvertEventArgs<BasketConversionPair>> Converting;

        /// <summary>
        /// Occurs after conversion.
        /// </summary>
        public static event TypedEventHandler<BasketConversionBase, ConvertEventArgs<BasketConversionPair>> Converted;

        /// <summary>
        /// Gets the original basket.
        /// </summary>
        protected IBasket AnonymousBasket { get; private set; }

        /// <summary>
        /// Gets the destination basket.
        /// </summary>
        protected IBasket CustomerBasket { get; private set; }

        /// <summary>
        /// Executes the merge basket strategy.
        /// </summary>
        /// <returns>
        /// The <see cref="IBasket"/>.
        /// </returns>
        public abstract IBasket Merge();

        /// <summary>
        /// Provides access to the Converting event.
        /// </summary>
        protected void OnConverting()
        {
            Converting.RaiseEvent(new ConvertEventArgs<BasketConversionPair>(GetBasketConversionPair(), false), this);
        }

        /// <summary>
        /// Provides access to the converted event.
        /// </summary>
        protected void OnConverted()
        {
            Converted.RaiseEvent(new ConvertEventArgs<BasketConversionPair>(GetBasketConversionPair(), false), this);
        }

        /// <summary>
        /// Creates a <see cref="BasketConversionPair"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="BasketConversionPair"/>.
        /// </returns>
        private BasketConversionPair GetBasketConversionPair()
        {
            return new BasketConversionPair
                       {
                           AnonymousBasket = this.AnonymousBasket,
                           CustomerBasket = this.CustomerBasket
                       };
        }
    }
}