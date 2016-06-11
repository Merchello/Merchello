namespace Merchello.Web.Store.Models
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a store view data result.
    /// </summary>
    public class StoreViewData : IMerchelloViewData
    {
        /// <summary>
        /// Gets or sets a value indicating whether success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public IEnumerable<string> Messages { get; set; }
    }
}