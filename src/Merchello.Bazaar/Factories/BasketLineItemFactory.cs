namespace Merchello.Bazaar.Factories
{
    using System;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Trees.Actions;

    using Umbraco.Core;
    using Umbraco.Web;

    /// <summary>
    /// Responsible for building <see cref="BasketLineItem"/>.
    /// </summary>
    internal class BasketLineItemFactory
    {
        private readonly MerchelloHelper _merchello = new MerchelloHelper();

        /// <summary>
        /// The _umbraco.
        /// </summary>
        private readonly UmbracoHelper _umbraco;

        /// <summary>
        /// The <see cref="ICustomerBase"/>.
        /// </summary>
        private readonly ICustomerBase _currentCustomer;

        /// <summary>
        /// The <see cref="ICurrency"/>
        /// </summary>
        private readonly ICurrency _currency;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketLineItemFactory"/> class.
        /// </summary>
        /// <param name="umbraco">
        /// The umbraco.
        /// </param>
        /// <param name="currentCustomer">
        /// The current Customer.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        public BasketLineItemFactory(UmbracoHelper umbraco, ICustomerBase currentCustomer, ICurrency currency)
        {
            Mandate.ParameterNotNull(umbraco, "umbraco");
            Mandate.ParameterNotNull(currency, "currency");
            Mandate.ParameterNotNull(currentCustomer, "currentCustomer");

            this._umbraco = umbraco;
            this._currency = currency;
            this._currentCustomer = currentCustomer;
        }

        /// <summary>
        /// Builds a <see cref="BasketLineItem"/>
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="BasketLineItem"/>.
        /// </returns>
        public BasketLineItem Build(ILineItem lineItem)
        {
            var contentId = lineItem.ExtendedData.ContainsKey("umbracoContentId")
                                ? int.Parse(lineItem.ExtendedData["umbracoContentId"])
                                : 0;

            var productKey = lineItem.ExtendedData.GetProductKey();
            ProductModel product = null;
            if (!productKey.Equals(Guid.Empty))
            {
                var productContent = _merchello.TypedProductContent(productKey);  
                if (productContent != null) product = new ProductModel(productContent)
                                                          {
                                                              CurrentCustomer = _currentCustomer,
                                                              Currency = _currency
                                                          };
            }
            

            var basketLineItem = new BasketLineItem
                {
                    Key = lineItem.Key,
                    ContentId = contentId,
                    Name = lineItem.Name,
                    Sku = lineItem.Sku,
                    UnitPrice = lineItem.Price,
                    TotalPrice = lineItem.TotalPrice,
                    Quantity = lineItem.Quantity,
                    ExtendedData = lineItem.ExtendedData,
                    Product = product ?? this.HandleLegacyBazaarProductContent(contentId)
                };

            return basketLineItem;
        }

        /// <summary>
        /// The handle legacy bazaar product content.
        /// </summary>
        /// <param name="contentId">
        /// The content id.
        /// </param>
        /// <returns>
        /// The <see cref="ProductModel"/>.
        /// </returns>
        private ProductModel HandleLegacyBazaarProductContent(int contentId)
        {
            return contentId > 0
                       ? new ProductModel(this._umbraco.TypedContent(contentId))
                             {
                                 CurrentCustomer =
                                     this._currentCustomer,
                                 Currency = this._currency
                             }
                       : null;
        }
    }
}