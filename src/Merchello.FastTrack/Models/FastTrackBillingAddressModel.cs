namespace Merchello.FastTrack.Models
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Localization;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// The checkout billing address model.
    /// </summary>
    public class FastTrackBillingAddressModel : FastTrackCheckoutAddressModel, ICustomerMembershipProfile
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelFirstName")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredFirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelLastName")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredLastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredEmailAddress")]
        [EmailAddress]
        public new string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use billing address for the shipping address.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelUseForShipping")]
        public bool UseForShipping { get; set; }
    }
}