namespace Merchello.Core.Chains.CopyEntity.Product
{
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Copies any product options.
    /// </summary>
    /// <remarks>
    /// Requires a save which will generate any product variants
    /// </remarks>
    internal sealed class CopyProductOptionsTask : CopyEntityTaskBase<IProduct>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProductOptionsTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        public CopyProductOptionsTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
        }

        /// <summary>
        /// Copies the options (if any).
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{T}"/>.
        /// </returns>
        public override Attempt<IProduct> PerformTask(IProduct entity)
        {
            if (!Original.DefinesOptions) return Attempt<IProduct>.Succeed(entity);

            foreach (var option in Original.ProductOptions)
            {
                if (option.Shared)
                {
                    entity.ProductOptions.Add(option.Clone());
                }
                else
                {
                    entity.ProductOptions.Add(new ProductOption(option.Name));
                    foreach (var choice in option.Choices)
                    {
                        entity.ProductOptions.First(x => x.Name == option.Name).Choices.Add(new ProductAttribute(choice.Name, choice.Sku));
                    }
                }
            }

            return Attempt<IProduct>.Succeed(entity);
        }
    }
}