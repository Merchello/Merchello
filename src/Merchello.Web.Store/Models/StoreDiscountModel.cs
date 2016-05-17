namespace Merchello.Web.Store.Models
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core.Localization;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a checkout discount model.
    /// </summary>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    public class StoreDiscountModel<TLineItemModel> : ICheckoutDiscountModel<TLineItemModel>
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelOfferCode")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredOfferCode")]
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDiscountViewData{TLineItemModel}"/>.
        /// </summary>
        public IDiscountViewData<TLineItemModel> ViewData { get; set; }
    }
}