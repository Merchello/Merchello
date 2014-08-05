namespace Merchello.Web.Workflow
{
    using Merchello.Core;

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
    }
}