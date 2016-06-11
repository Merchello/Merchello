namespace Merchello.Web.Factories
{
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Responsible for creating <see cref="IAddItemModel"/>s.
    /// </summary>
    /// <typeparam name="TAddItemModel">
    /// The type of the add item model
    /// </typeparam>
    public class AddItemModelFactory<TAddItemModel>
        where TAddItemModel : class, IAddItemModel, new()
    {
        /// <summary>
        /// Creates <see cref="IAddItemModel"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="productContent">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <returns>
        /// The <see cref="IAddItemModel"/>.
        /// </returns>
        public TAddItemModel Create(IProductContent productContent, int quantity = 1)
        {
            var addItem = this.Create(productContent.AsProductDisplay(), quantity);
            return this.OnCreate(addItem, productContent);
        }

        /// <summary>
        /// Creates <see cref="IAddItemModel"/> from <see cref="ProductDisplay"/>.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <returns>
        /// The <see cref="IAddItemModel"/>.
        /// </returns>
        public TAddItemModel Create(ProductDisplay display, int quantity = 1)
        {
            var addItem = new TAddItemModel
                {
                    ProductKey = display.Key,
                    Quantity = quantity,
                    ProductOptions = display.ProductOptions
                };

            return this.OnCreate(addItem, display);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="IAddItemModel"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="addItem">
        /// The <see cref="IAddItemModel"/>.
        /// </param>
        /// <param name="productContent">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="IAddItemModel"/>.
        /// </returns>
        protected virtual TAddItemModel OnCreate(TAddItemModel addItem, IProductContent productContent)
        {
            return addItem;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="IAddItemModel"/> from <see cref="ProductDisplay"/>
        /// </summary>
        /// <param name="addItem">
        /// The <see cref="IAddItemModel"/>.
        /// </param>
        /// <param name="display">
        /// The <see cref="ProductDisplay"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="IAddItemModel"/>.
        /// </returns>
        protected virtual TAddItemModel OnCreate(TAddItemModel addItem, ProductDisplay display)
        {
            return addItem;
        }
    }
}