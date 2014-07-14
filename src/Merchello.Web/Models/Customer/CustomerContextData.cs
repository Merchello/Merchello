namespace Merchello.Web.Models.Customer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The customer context data.
    /// </summary>
    public class CustomerContextData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContextData"/> class.
        /// </summary>
        public CustomerContextData()
        {
            ContextValues = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets the context values.
        /// </summary>
        public List<KeyValuePair<string, string>> ContextValues { get; private set; } 
    }
}