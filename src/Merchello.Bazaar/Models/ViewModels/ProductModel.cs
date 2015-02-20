namespace Merchello.Bazaar.Models.ViewModels
{
    using System.Web;

    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    /// <summary>
    /// Represents a product model.
    /// </summary>
    public class ProductModel : MasterModel
    {
        /// <summary>
        /// The image.
        /// </summary>
        private string _image;

        /// <summary>
        /// The brief text for the product group
        /// </summary>
        private string _brief;

        /// <summary>
        /// The description.
        /// </summary>
        private IHtmlString _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public ProductModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets the product data.
        /// </summary>
        public ProductDisplay ProductData
        {
            get
            {
                return this.Content.GetPropertyValue<ProductDisplay>("merchelloProduct");
            }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public string Image
        {
            get
            {
                if (this._image == null && this.Content.HasValue("image"))
                {
                    this._image = this.Content.GetCropUrl(propertyAlias: "image", imageCropMode: ImageCropMode.Max);
                }

                return this._image;
            }
        }

        /// <summary>
        /// Gets the brief.
        /// </summary>
        public string Brief
        {
            get
            {
                return this._brief ?? this.Content.GetPropertyValue<string>("brief");
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public IHtmlString Description
        {
            get
            {
                return this._description ?? this.Content.GetPropertyValue<IHtmlString>("description");
            }
        }
    }
}