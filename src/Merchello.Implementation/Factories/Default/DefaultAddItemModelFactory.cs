namespace Merchello.Implementation.Factories
{
    using Merchello.Implementation.Models;
    using Web.Models.VirtualContent;

    /// <summary>
    /// A factory responsible for creating <see cref="AddItemModel"/> in default implementations.
    /// </summary>
    public class DefaultAddItemModelFactory : AddItemModelFactory<AddItemModel>
    {
        /// <summary>
        ///  Allows for overriding the creation of <see cref="AddItemModel"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="addItem">
        /// The <see cref="AddItemModel"/>.
        /// </param>
        /// <param name="productContent">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="AddItemModel"/>.
        /// </returns>
        protected override AddItemModel OnCreate(AddItemModel addItem, IProductContent productContent)
        {
            // Set the success URL to the product page
            addItem.SuccessRedirectUrl = productContent.Url;

            return base.OnCreate(addItem, productContent);
        }
    }
}