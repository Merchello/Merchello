namespace Merchello.Web.Models.ContentEditing.Checkout
{
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

        /// <summary>
        /// Gets or sets the item cache type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemCacheType ItemCacheType { get; set; }

    }
}