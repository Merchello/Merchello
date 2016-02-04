namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Represents the store model.
    /// </summary>
    public partial class StoreModel : MasterModel
    {
        private List<ProductBoxModel> _productModels;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public StoreModel(IPublishedContent content)
            : base(content)
        {
        }

        public List<ProductBoxModel> FeaturedProducts
        {
            get
            {
                if (_productModels == null || !_productModels.Any())
                {
                    var productList = this.Content.GetPropertyValue<IEnumerable<IProductContent>>("featuredProducts").ToList();
                    _productModels = new List<ProductBoxModel>();
                    if (productList.Any())
                    {
                        _productModels = BazaarContentHelper.GetProductBoxModels(productList);
                    }
                }                
                return _productModels;
            }
        } 

        public string Overview
        {
            get
            {
                return this.Content.GetPropertyValue<string>("overview");
            }
        }

        /// <summary>
        /// Gets the tag line.
        /// </summary>
        [Obsolete("This is no longer used")]
        public string TagLine 
        { 
            get
            {
                return this.Content.GetPropertyValue<string>("tagLine");
            } 
        }
    }
}