namespace Merchello.Web.Ui.Implementation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a basket item UI component.
    /// </summary>
    public interface IBasketItemModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        IEnumerable<Tuple<string, string>> CustomerOptionChoices { get; set; }
    }
}