namespace Merchello.Core.Chains.CopyEntity.Product
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;

    using Umbraco.Core;

    /// <summary>
    /// The copy detached content task.
    /// </summary>
    internal sealed class CopyDetachedContentTask : CopyProductTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDetachedContentTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="original">
        /// The original.
        /// </param>
        public CopyDetachedContentTask(IMerchelloContext merchelloContext, IProduct original)
            : base(merchelloContext, original)
        {
        }

        /// <summary>
        /// The perform task.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IProduct> PerformTask(IProduct value)
        {
            // copy the product detached content
            foreach (var originalDetachedContent in Original.DetachedContents.ToArray())
            {
                value.DetachedContents.Add(this.BuildDetachedContent(value.ProductVariantKey, value.Name, originalDetachedContent));
            }

            // copy each variant content if they have it
            foreach (var originVariant in Original.ProductVariants)
            {
                if (originVariant.DetachedContents.Any())
                {
                    var matchingClonedVariant = this.GetClonedMathingVariant(value, originVariant);
                    foreach (var originalVariantContent in originVariant.DetachedContents.ToArray())
                    {
                        matchingClonedVariant.DetachedContents.Add(this.BuildDetachedContent(matchingClonedVariant.Key, matchingClonedVariant.Name, originalVariantContent));
                    }
                }
            }

            return Attempt<IProduct>.Succeed(value);
        }

        /// <summary>
        /// The build detached content.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="originalDetachedContent">
        /// The original detached content.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariantDetachedContent"/>.
        /// </returns>
        private IProductVariantDetachedContent BuildDetachedContent(
            Guid productVariantKey,
            string name,
            IProductVariantDetachedContent originalDetachedContent)
        {
           return new ProductVariantDetachedContent(
                productVariantKey,
                originalDetachedContent.DetachedContentType,
                originalDetachedContent.CultureName, 
                new DetachedDataValuesCollection(originalDetachedContent.DetachedDataValues.ToArray()))
                                      {
                                          Slug = PathHelper.ConvertToSlug(name),
                                          CanBeRendered = originalDetachedContent.CanBeRendered,
                                          TemplateId = originalDetachedContent.TemplateId,
                                      };
        }
    }
}