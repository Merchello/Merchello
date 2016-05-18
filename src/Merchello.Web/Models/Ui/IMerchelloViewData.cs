namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a Merchello View Data Result.
    /// </summary>
    public interface IMerchelloViewData
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets any exception that occurred during the operation attempt.
        /// </summary>
        /// <remarks>
        /// Defaults is null
        /// </remarks>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the collection of messages resulting from the operation attempt.
        /// </summary>
        IEnumerable<string> Messages { get; set; }
    }
}