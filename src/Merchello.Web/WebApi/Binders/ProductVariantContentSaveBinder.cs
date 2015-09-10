namespace Merchello.Web.WebApi.Binders
{
    using Merchello.Web.Models.ContentEditing.Content;

    using Umbraco.Core;

    /// <summary>
    /// The product variant content save binder.
    /// </summary>
    internal class ProductVariantContentSaveBinder : DetachedContentSaveBinder<ProductVariantContentSave>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantContentSaveBinder"/> class.
        /// </summary>
        public ProductVariantContentSaveBinder()
            : base(ApplicationContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantContentSaveBinder"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        public ProductVariantContentSaveBinder(ApplicationContext applicationContext)
            : base(applicationContext)
        {
        }
    }
}