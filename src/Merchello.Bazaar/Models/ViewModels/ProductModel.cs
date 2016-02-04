namespace Merchello.Bazaar.Models.ViewModels
{
    using System.Web;

    using Merchello.Web.Models.ContentEditing;
    using Merchello.Examine.DataServices;

    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    /// <summary>
    /// Represents a product model.
    /// </summary>
    public partial class ProductModel : MasterModel
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
        /// Product Data
        /// </summary>
        private ProductDisplay _productDisplay;

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
                if (_productDisplay == null)
                {
                    var fromProduct = this.Content.GetPropertyValue<ProductDisplay>("merchelloProduct");
                    if (fromProduct != null)
                    {
                        _productDisplay = fromProduct;
                    }
                    else
                    {
                        _productDisplay = new ProductDisplay();
                    }
                }
                return _productDisplay;
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
                    this._image = this.Content.GetCropUrl(propertyAlias: "image", imageCropMode: ImageCropMode.Max, cacheBuster: false);
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
                if (string.IsNullOrEmpty(this._brief))
                {
                    this._brief = this.Content.GetPropertyValue<string>("brief");
                }
                return this._brief;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public IHtmlString Description
        {
            get
            {
                if (this._description == null && this.Content.HasValue("description"))
                {
                    this._description = this.Content.GetPropertyValue<IHtmlString>("description");
                }
                return this._description;
            }
        }
    }
}