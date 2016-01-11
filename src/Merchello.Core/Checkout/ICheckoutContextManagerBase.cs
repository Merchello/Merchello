namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines a checkout context manager.
    /// </summary>
    public interface ICheckoutContextManagerBase
    {
        /// <summary>
        /// Gets the <see cref="ICheckoutContext"/>.
        /// </summary>
        ICheckoutContext Context { get; }
    }
}