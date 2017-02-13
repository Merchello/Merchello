namespace Merchello.Core
{
    using Merchello.Core.Models;

    /// <summary>
    /// Extension methods for <see cref="IProductOption"/>.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Adds a new option choice.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public static void AddChoice(this IProductOption option, string name, string sku)
        {
            option.Choices.Add(new ProductAttribute(name, sku));
        }
    }
}
