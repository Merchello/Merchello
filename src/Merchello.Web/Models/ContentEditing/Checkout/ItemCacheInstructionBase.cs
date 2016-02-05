namespace Merchello.Web.Models.ContentEditing.Checkout
{
    using System.Runtime.Serialization;

    using Merchello.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The item cache instruction base.
    /// </summary>
    public abstract class ItemCacheInstructionBase
    {
        /// <summary>
        /// Gets or sets the <see cref="CustomerDisplay"/>.
        /// </summary>
        public CustomerDisplay Customer { get; set; }
    }
}