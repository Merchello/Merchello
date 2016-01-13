namespace Merchello.Bazaar.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    /// <summary>
    /// Represents a ProductGroup view model.
    /// </summary>
    public partial class ProductGroupModel : MasterModel
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
        /// Child products.
        /// </summary>
        private IEnumerable<ProductModel> _childProducts; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductGroupModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public ProductGroupModel(IPublishedContent content)
            : base(content)
        {
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
        /// Gets the brief text for the product group.
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
        /// Gets the children.
        /// </summary>
        public override IEnumerable<IPublishedContent> Children
        {
            get
            {
                if (this._childProducts != null) return this._childProducts;
                this._childProducts = base.Children.Select(x => new ProductModel(x)
                                                               {
                                                                   CurrentCustomer = this.CurrentCustomer,
                                                                   Currency = this.Currency
                                                               });
                return this._childProducts;
            }
        }
    }
}