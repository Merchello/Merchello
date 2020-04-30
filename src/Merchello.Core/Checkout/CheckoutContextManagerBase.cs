namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// A base class for managers that require the <see cref="ICheckoutContext"/>.
    /// </summary>
    public abstract class CheckoutContextManagerBase : ICheckoutContextManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutContextManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutContextManagerBase(ICheckoutContext context)
        {
            Ensure.ParameterNotNull(context, "context");

            this.Context = context;
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutContext"/>.
        /// </summary>
        public ICheckoutContext Context { get; private set; }

        /// <summary>
        /// Resets (removes) data.
        /// </summary>
        public abstract void Reset();
    }
}