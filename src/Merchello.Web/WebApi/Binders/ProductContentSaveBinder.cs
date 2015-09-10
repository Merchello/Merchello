namespace Merchello.Web.WebApi.Binders
{
    using Merchello.Web.Models.ContentEditing.Content;

    using Umbraco.Core;

    /// <summary>
    /// The product content save binder.
    /// </summary>
    internal class ProductContentSaveBinder : DetachedContentSaveBinder<ProductContentSave>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentSaveBinder"/> class.
        /// </summary>
        public ProductContentSaveBinder()
            : base(ApplicationContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentSaveBinder"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        public ProductContentSaveBinder(ApplicationContext applicationContext)
            : base(applicationContext)
        {
        }
    }
}