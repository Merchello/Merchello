namespace Merchello.Core.Models
{
    using Merchello.Core.Services;

    /// <summary>
    /// Extension methods for <see cref="IProductOption"/>.
    /// </summary>
    public static class ProductOptionExtensions
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