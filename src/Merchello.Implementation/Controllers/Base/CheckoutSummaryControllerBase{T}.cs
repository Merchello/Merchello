namespace Merchello.Implementation.Controllers.Base
{
    using Merchello.Implementation.Factories;

    /// <summary>
    /// A base controller for checkout summary rendering and operations.
    /// </summary>
    /// <typeparam name="TSummary">
    /// The type of the checkout summary
    /// </typeparam>
    public abstract class CheckoutSummaryControllerBase<TSummary> : CheckoutControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary}"/> class.
        /// </summary>
        protected CheckoutSummaryControllerBase()
            : this(new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryControllerBase{TSummary}"/> class.
        /// </summary>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutSummaryControllerBase(CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
        }
    }
}