namespace Merchello.Web.Workflow.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Sales;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    using Constants = Merchello.Core.Constants;

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