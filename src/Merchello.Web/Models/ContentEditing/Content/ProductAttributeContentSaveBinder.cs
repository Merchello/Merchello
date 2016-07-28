namespace Merchello.Web.Models.ContentEditing.Content
{
    using Merchello.Web.WebApi.Binders;

    using Umbraco.Core;

    /// <summary>
    /// The product attribute content save binder.
    /// </summary>
    public class ProductAttributeContentSaveBinder : DetachedContentSaveBinder<ProductAttributeContentSave>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributeContentSaveBinder"/> class.
        /// </summary>
        public ProductAttributeContentSaveBinder()
            : this(ApplicationContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributeContentSaveBinder"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        public ProductAttributeContentSaveBinder(ApplicationContext applicationContext)
            : base(applicationContext)
        {
        }
    }
}