namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Events;

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