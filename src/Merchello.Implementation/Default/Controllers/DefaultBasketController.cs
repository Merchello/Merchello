namespace Merchello.Implementation.Default.Controllers
{
    using System.Web.Mvc;

    using Merchello.Implementation.Attributes;
    using Merchello.Implementation.Controllers;
    using Merchello.Implementation.Default.Models;
    using Merchello.Implementation.Factories;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default (generic) basket controller.
    /// </summary>
    [PluginController("Merchello")]
    [ComponentSetAlias("Default")]
    public class DefaultBasketController : BasketControllerBase<BasketModel, BasketItemModel, AddItemModel>
    {
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
        /// Handles the redirection after successfully adding an item to the basket.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult RedirectAddItemSuccess(AddItemModel model)
        {
            return this.Redirect(model.SuccessRedirectUrl);
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