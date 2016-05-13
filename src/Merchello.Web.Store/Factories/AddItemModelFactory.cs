namespace Merchello.Web.Store.Factories
{
    using Merchello.Web.Factories;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A factory responsible for creating <see cref="StoreAddItemModel"/> in default implementations.
    /// </summary>
    public class AddItemModelFactory : AddItemModelFactory<StoreAddItemModel>
    {
        /// <summary>
        ///  Allows for overriding the creation of <see cref="StoreAddItemModel"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="addItem">
        /// The <see cref="StoreAddItemModel"/>.
        /// </param>
        /// <param name="productContent">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="StoreAddItemModel"/>.
        /// </returns>
        protected override StoreAddItemModel OnCreate(StoreAddItemModel addItem, IProductContent productContent)
        {
            // Set the success URL to the product page
            addItem.SuccessRedirectUrl = productContent.Url;

            return base.OnCreate(addItem, productContent);
        }
    }
}