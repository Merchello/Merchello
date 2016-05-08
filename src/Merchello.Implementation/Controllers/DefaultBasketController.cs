namespace Merchello.Implementation.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Models;
    using Merchello.Implementation.Controllers.Base;
    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;
    using Merchello.Web;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default (generic) basket controller.
    /// </summary>
    [PluginController("Merchello")]
    public class DefaultBasketController : BasketControllerBase<BasketModel, BasketItemModel, AddItemModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello = new MerchelloHelper();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBasketController"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is actually required for this controller since we are not actually
        /// overriding the default BasketItemExtendedDataFactory, but is here to provide and example
        /// of how it would be done.  To override, create a factory that inherits from BasketItemExtendedDataFactory
        /// and pass it into the base constructor.
        /// </remarks>
        public DefaultBasketController()
            : base(new BasketItemExtendedDataFactory<AddItemModel>())
        {
        }

        /// <summary>
        /// Maps a <see cref="ILineItem"/> to <see cref="IBasketItemModel"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="IBasketItemModel"/>.
        /// </returns>
        //// TODO put in factory
        protected override BasketItemModel MapLineItemToBasketLineItem(ILineItem lineItem)
        {
            var productKey = lineItem.ExtendedData.GetProductKey();
            IProductContent product = null;
            if (!productKey.Equals(Guid.Empty)) product = this._merchello.TypedProductContent(productKey);

            return new BasketItemModel
            {
                Key = lineItem.Key,
                Name = lineItem.Name,
                ProductKey = productKey,
                Product = product,
                Amount = lineItem.Price,
                Quantity = lineItem.Quantity,
                CustomerOptionChoices = lineItem.GetProductOptionChoicePairs()
            };
        }

        /// <summary>
        /// Maps <see cref="IProductContent"/> to <see cref="AddItemModel"/>.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The mapped <see cref="AddItemModel"/> object.
        /// </returns>
        //// TODO put in factory
        protected override AddItemModel MapProductContentToAddItemModel(IProductContent product)
        {
            // We map every property except OptionChoices which are used in the post back to determine
            // the selected variant (if any)
            return new AddItemModel
                       {
                           ProductKey = product.Key,
                           ProductOptions = product.ProductOptions,
                           Quantity = 1,
                           SuccessRedirectUrl = product.Url
                       };
        }
    }
}