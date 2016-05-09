namespace Merchello.Web.Factories
{
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A factory responsible for building an <see cref="ExtendedDataCollection"/> for basket Line Items.
    /// </summary>
    /// <typeparam name="TAddItemModel">
    /// The type of <see cref="IAddItemModel"/>
    /// </typeparam>
    /// <remarks>
    /// This allows for custom (site specific) values being added to the <see cref="ExtendedDataCollection"/> by
    /// overriding the "OnCreate" virtual method and storing values that may be added to models implementing
    /// <see cref="IAddItemModel"/>
    /// </remarks>
    public class BasketItemExtendedDataFactory<TAddItemModel>
        where TAddItemModel : class, IAddItemModel, new()
    {
        /// <summary>
        /// Builds an <see cref="ExtendedDataCollection"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </returns>
        public ExtendedDataCollection Create(TAddItemModel model)
        {
            return this.OnCreate(new ExtendedDataCollection(), model);
        }

        /// <summary>
        /// An overridable method that allows for adding custom extended data values to the line item that is to be created
        /// and added to the basket.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </returns>
        /// <remarks>
        /// Custom values will be copied to the <see cref="IInvoice"/> upon completion of the sale
        /// </remarks>
        protected virtual ExtendedDataCollection OnCreate(ExtendedDataCollection extendedData, TAddItemModel model)
        {
            return extendedData;
        }
    }
}