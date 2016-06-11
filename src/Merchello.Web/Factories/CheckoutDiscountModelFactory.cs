namespace Merchello.Web.Factories
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A factory responsible for building <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
    /// </summary>
    /// <typeparam name="TDiscountModel">
    /// The type of <see cref="ICheckoutDiscountModel{TLineItemModel}"/>
    /// </typeparam>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    public class CheckoutDiscountModelFactory<TDiscountModel, TLineItemModel>
        where TLineItemModel : class, ILineItemModel, new()
        where TDiscountModel : class, ICheckoutDiscountModel<TLineItemModel>, new()
    {
        /// <summary>
        /// Creates a <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </returns>
        public TDiscountModel Create()
        {
            var discount = new TDiscountModel();

            return OnCreate(discount);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </summary>
        /// <param name="remptionResult">
        /// The <see cref="IOfferRedemptionResult{ILineItem}"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </returns>
        public TDiscountModel Create(IOfferRedemptionResult<ILineItem> remptionResult)
        {
            // We're leaving DiscountApplicationResult as an internal class
            // so that it is not inadvertingly instantiated.
            var result = new DiscountViewData<TLineItemModel>
                             {
                                 Success = remptionResult.Success,
                                 LineItem = remptionResult.Success ?
                                    new TLineItemModel
                                    {
                                        Name = remptionResult.Award.Name,
                                        Sku = remptionResult.Award.Sku,
                                        Quantity = remptionResult.Award.Quantity,
                                        Amount = remptionResult.Award.Price,
                                        LineItemType = remptionResult.Award.LineItemType
                                    } 
                                    : null,
                                 Exception = remptionResult.Exception,
                                 Messages = remptionResult.Messages
                             };

            var discount = new TDiscountModel { ViewData = result };

            return OnCreate(discount, remptionResult.Award);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </summary>
        /// <param name="discount">
        /// The <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </returns>
        public TDiscountModel OnCreate(TDiscountModel discount)
        {
            return discount;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </summary>
        /// <param name="discount">
        /// The <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </param>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutDiscountModel{TLineItemModel}"/>.
        /// </returns>
        public TDiscountModel OnCreate(TDiscountModel discount, ILineItem lineItem)
        {
            return discount;
        }
    }
}