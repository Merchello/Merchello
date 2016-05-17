namespace Merchello.Web.Store.Models
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core.Localization;
    using Merchello.Web.Store.Validation;

    /// <summary>
    /// A model for rendering and processing .
    /// </summary>
    public class BraintreeCcPaymentModel : BraintreePaymentModel
    {
        /// <summary>
        /// Gets or sets the card holder.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredCardHolderName")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelCardHolderName")]
        public string CardHolder { get; set; }

        /// <summary>
        /// Gets or sets the credit card number.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredCreditCardNumber")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelCreditCardNumber")]
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the expires month year.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelExpirationDate")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredExpirationDate")]
        [ValidateExpiresDate(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "InvalidCcExpiresDate")]
        public string ExpiresMonthYear { get; set; }

        /// <summary>
        /// Gets or sets the CVV.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredCvv")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelCvv")]
        [MinLength(3, ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "InvalidCvv")]
        [MaxLength(4, ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "InvalidCvv")]
        public string Cvv { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredPostalCode")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelPostalCode")]
        public string PostalCode { get; set; }
    }
}