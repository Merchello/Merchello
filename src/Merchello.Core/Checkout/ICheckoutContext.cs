﻿namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core.Cache;
    using Umbraco.Core.Events;

    /// <summary>
    /// Defines a checkout context.
    /// </summary>
    public interface ICheckoutContext
    {
        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>.
        /// </summary>
        IMerchelloContext MerchelloContext { get; }

        /// <summary>
        /// Gets the <see cref="IServiceContext"/>.
        /// </summary>
        IServiceContext Services { get; }

        /// <summary>
        /// Gets the <see cref="IGatewayContext"/>.
        /// </summary>
        IGatewayContext Gateways { get; }

        /// <summary>
        /// Gets the <see cref="IItemCache"/>.
        /// </summary>
        /// <remarks>
        /// This is a temporary collection of line items that is copied from the basket that can be modified
        /// while preparing the final invoice.
        /// </remarks>
        IItemCache ItemCache { get; }

        /// <summary>
        /// Gets the customer associated with the checkout.
        /// </summary>
        ICustomerBase Customer { get; }

        /// <summary>
        /// Gets the checkout version key.
        /// </summary>
        Guid VersionKey { get; }

        /// <summary>
        /// Gets a value indicating whether this context is a new checkout version.
        /// </summary>
        bool IsNewVersion { get; }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        IRuntimeCacheProvider Cache { get; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        ICheckoutContextSettings Settings { get; }
    }
}