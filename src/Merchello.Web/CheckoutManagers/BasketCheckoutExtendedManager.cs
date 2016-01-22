namespace Merchello.Web.CheckoutManagers
{
    using Merchello.Core.Checkout;

    /// <summary>
    /// Represents an extended checkout manager for basket checkouts.
    /// </summary>
    internal class BasketCheckoutExtendedManager : CheckoutExtendedManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutExtendedManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public BasketCheckoutExtendedManager(ICheckoutContext context)
            : base(context)
        {
        }
    }
}